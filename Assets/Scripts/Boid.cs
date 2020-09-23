using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoidFactory bfactory;
    private SpriteRenderer sr;

    private float speed;

    private bool initialized = false;
    private int boidLayer;
    private PlayerController player;

    private bool isShot;
    private bool canSeePlayer;
    private bool canSeeMonster;

    private enum State { Roam, Follow, Evade, Shot};
    private State currentState;


    public void Initialize(float speed, Vector2 pos, Vector2 dir, BoidFactory bfactory)
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        this.bfactory = bfactory;
        this.speed = speed;

        transform.position = pos;
        rb.velocity = dir * speed;
        currentState = State.Roam;

        isShot = false;
        canSeePlayer = false;
        initialized = true;
    }

    void FixedUpdate()
    {
        if(initialized)
        {
            FlipSprite();

            switch(currentState)
            {
                case State.Roam:
                    CheckForObstacles();
                    LimitVelocity();
                    ApplyRules();
                    //BoundVelocity();
                    break;
                case State.Follow:
                    CheckForObstacles();
                    LimitVelocity();
                    ApplyRules();
                    MoveTowardsPlayer();
                    CheckDistanceFromPlayer();
                    break;
               // case State.Shot:


            }
        }
    }

    void Update()
    {
        CheckState();
    }

    void FlipSprite()
    {
        sr.flipX = (rb.velocity.x < 0);
    }

    void CheckState()
    {
        switch(currentState)
        {
            case State.Roam:
                if (isShot) currentState = State.Shot;
                else if (canSeePlayer) currentState = State.Follow;
                else if (canSeeMonster) currentState = State.Evade;
                else currentState = State.Roam;
                break;

            case State.Follow:
                if (isShot) currentState = State.Shot;
                else if (canSeePlayer) currentState = State.Follow;
                else if (canSeeMonster) currentState = State.Evade;
                else currentState = State.Roam;
                break;

            case State.Evade:
                if (isShot) currentState = State.Shot;
                else if (canSeePlayer) currentState = State.Follow;
                else if (canSeeMonster) currentState = State.Evade;
                else currentState = State.Roam;
                break;

            case State.Shot:
                if (isShot) currentState = State.Shot;
                else if (canSeePlayer) currentState = State.Follow;
                else if (canSeeMonster) currentState = State.Evade;
                else currentState = State.Roam;
                break;
        }
    }

    void ApplyRules()
    {
        boidLayer = 1 << LayerMask.NameToLayer("Boid"); 
        Collider2D[] closeBoids = Physics2D.OverlapCircleAll(transform.position, bfactory.GetRange(), boidLayer);   // Get all boids in the range

        MoveWith(closeBoids);
        MoveTowardsCenter(closeBoids);
        MoveAway(closeBoids);
    }

    void MoveWith(Collider2D[] closeBoids)
    {
        float size = 0f;
        Vector2 avgVelocity = Vector2.zero;

        for(int i = 0; i < closeBoids.Length; i++)          // For every close boid
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if(boid != this && boid.currentState != State.Shot)
            {
                avgVelocity += boid.GetVelocity();          // Calculate average velocity
                size++;
            }
        }

        if(size > 0)
        {
            avgVelocity /= size;
            rb.velocity += (avgVelocity * (bfactory.GetAlignmentFactor() / 10.0f));     // Add an amount of average velocity calculated to the current velocity 
        }
    }

    void MoveTowardsCenter(Collider2D[] closeBoids)
    {
        float size = 0f;
        Vector2 avgPosition = Vector2.zero;

        for (int i = 0; i < closeBoids.Length; i++)         // For every close boid
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if (boid != this && boid.currentState != State.Shot)
            {
                avgPosition += boid.GetPosition();          // Calculate average position
                size++;
            }
        }

        if (size > 0)
        {
            avgPosition /= size;

            Vector2 targetPosition = avgPosition - (Vector2)transform.position;        // Get the direction from current position to the average position calculated and normalize it
            targetPosition = targetPosition.normalized;

            rb.velocity += targetPosition * (bfactory.GetCohesionFactor() / 10.0f);   // Add an amount of 
        }
    }

    void MoveAway(Collider2D[] closeBoids)
    {
        float theta = Mathf.Deg2Rad * 45f;

        float cs = Mathf.Cos(theta);
        float sn = Mathf.Sin(theta);

        for (int i = 0; i < closeBoids.Length; i++)
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if (boid != this && boid.currentState != State.Shot)
            {
                Vector2 targetPosition = (Vector2)transform.position - boid.GetPosition();
                targetPosition = targetPosition.normalized;

                float px = targetPosition.x * cs - targetPosition.y * sn;
                float py = targetPosition.x * sn + targetPosition.y * cs;

                Vector2 dir = new Vector2(px, py);
                rb.velocity += dir * (bfactory.GetSeparationFactor() / 10.0f);
            }
        }
    }

    void CheckForObstacles()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, bfactory.GetRange() * 3f, LayerMask.GetMask("Obstacle"));

        if(hit.collider != null)        // If it hits an obstacle
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, Time.deltaTime);
            AvoidObstacle();
        }
    }

    void AvoidObstacle()
    {
        float theta_p =  Mathf.PI / 8f;
        float theta_m = -Mathf.PI / 8f;

        for (int i = 0; i <= 8; i++)
        {
            Vector2 dir = rb.velocity.normalized;
            Vector2 dirToCast;

            dirToCast = CastRay(theta_p * i, dir);
            if (dirToCast != Vector2.zero)
            {
                Debug.DrawLine(transform.position, (Vector2)transform.position + (dirToCast * bfactory.GetRange() * 3f), Color.green, Time.deltaTime);
                rb.velocity = dirToCast * rb.velocity.magnitude;// * 0.7f) + (rb.velocity * 0.3f);
                break;
            }

            dirToCast = CastRay(theta_m * i, dir);
            if (dirToCast != Vector2.zero)
            {
                Debug.DrawLine(transform.position, (Vector2)transform.position + (dirToCast * bfactory.GetRange() * 3f), Color.green, Time.deltaTime);
                rb.velocity = dirToCast * rb.velocity.magnitude;// * 0.7f) + (rb.velocity * 0.3f);
                break;
            }
        }
    }


    Vector2 CastRay(float theta_i, Vector2 dir)
    {
        float cs = Mathf.Cos(theta_i);
        float sn = Mathf.Sin(theta_i);

        float px = dir.x * cs - dir.y * sn;
        float py = dir.x * sn + dir.y * cs;
        Vector2 dirToCast = new Vector2(px, py);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToCast, bfactory.GetRange() * 3f, LayerMask.GetMask("Obstacle"));

        if (hit.collider == null)        // If it does not hit an obstacle
        {
            return dirToCast;
        }
        else
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, Time.deltaTime);
            return Vector2.zero;
        }
    }

    void LimitVelocity()
    {
        if(rb.velocity.magnitude > bfactory.GetMaxVelocity())
        {
            rb.velocity *= bfactory.GetVelLimitFactor();
        }
    }

    public void StartFollowing(PlayerController player)
    {
        if (currentState != State.Shot)
        {
            this.player = player;
            canSeePlayer = true;
        }
    }

    public void MoveTowardsPlayer()
    {
        Vector2 dir = player.GetPosition() - (Vector2)transform.position;

        if(dir.magnitude <= player.GetRange())
        {
            dir = dir.normalized;
            rb.velocity += (dir * player.GetAttractForce());
        }
        else
        {
            canSeePlayer = false;
        }
    }

    void CheckDistanceFromPlayer()
    {
        float dst = ((Vector2)transform.position - player.GetPosition()).magnitude;
        if(dst > player.GetRange())
        {
            canSeePlayer = false;
        }
    }

    void StopShooting()
    {
        if (currentState == State.Shot)
        {
            isShot = false;
            float rvx = Random.Range(1f, 4f);
            float rvy = Random.Range(1f, 4f);
            rb.velocity = new Vector2(rvx, rvy);
        }
    }

    public void ShotAt(Vector2 playerPosition, Vector2 shootDirection, float shootForce)
    {
        if(currentState != State.Shot)
        {
            isShot = true;
            canSeePlayer = false;
            canSeeMonster = false;

            rb.velocity = Vector2.zero;
            float dtForce = shootForce;
            float shotTime = 0f;
            Vector2 targetPosition = playerPosition + shootDirection * shootForce * 0.005f;
            Vector2 directionFromBoid = (targetPosition - (Vector2)transform.position).normalized;
            float distFromTarget = (targetPosition - (Vector2)transform.position).magnitude;

            StartCoroutine(ShotTravel(targetPosition, directionFromBoid, shootForce, dtForce, distFromTarget, shotTime));
        }
    }

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    IEnumerator ShotTravel(Vector2 targetPosition, Vector2 directionFromBoid, float shootForce, float dtForce, float distFromTarget, float shotTime)
    {
        float dst;
        do
        {
            rb.velocity = directionFromBoid * dtForce * Time.deltaTime;

            dst = (targetPosition - (Vector2)transform.position).magnitude / distFromTarget;
            dtForce = shootForce * dst;
            shotTime += Time.deltaTime;

            yield return null;
        } while (dst > 0.1f && shotTime < 3f && isShot);

        StopShooting();

        yield return null;
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.collider.CompareTag("Obstacle"))
        {
            StopShooting();
        }
    }

    //void OnDrawGizmos()
    //{
    //    if(initialized)
    //    {
    //        Gizmos.DrawWireSphere(transform.position, bfactory.GetRange());
    //    }
    //}

}
