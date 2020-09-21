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
    private bool isShot;


    public void Initialize(float speed, Vector2 pos, Vector2 dir, BoidFactory bfactory)
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        this.bfactory = bfactory;
        this.speed = speed;

        transform.position = pos;
        rb.velocity = dir * speed;

        isShot = false;
        initialized = true;
    }

    void FixedUpdate()
    {
        if(initialized)
        {
            FlipSprite();

            if (!isShot)
            {
                CheckForObstacles();
                LimitVelocity();
                ApplyRules();
                //BoundVelocity();
            }
        }
    }

    void FlipSprite()
    {
        sr.flipX = (rb.velocity.x < 0);
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
            if(boid != this)
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
            if (boid != this)
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
            if (boid != this)
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

    void BoundVelocity()
    {
        float boundX = bfactory.GetBoundX();
        float boundY = bfactory.GetBoundY();

        if(transform.position.x < -boundX)
        {
            transform.position = new Vector2(boundX, transform.position.y);
        }
        else if(transform.position.x > boundX)
        {
            transform.position = new Vector2(-boundX, transform.position.y);
        }

        if (transform.position.y < -boundY)
        {
            transform.position = new Vector2(transform.position.x, boundY);
        }
        else if (transform.position.y > boundY)
        {
            transform.position = new Vector2(transform.position.x, -boundY);
        }
    }

    public void MoveTowardsPlayer(Vector2 playerPosition, float attractForce)
    {
        if(!isShot)
        {
            Vector2 dir = playerPosition - (Vector2)transform.position;
            dir = dir.normalized;
            rb.velocity += (dir * attractForce);
        }
    }

    public void ShotAt(Vector2 direction, float shootForce)
    {
        if(!isShot)
        {
            //StartCoroutine(ShotTravel(direction, shootForce));
            isShot = true;
            Debug.Log(isShot);
            rb.velocity = Vector2.zero;

            while ((Vector2)transform.position != direction)
            {
                transform.position = Vector2.MoveTowards(transform.position, direction, shootForce * Time.deltaTime);
            }

            //yield return new WaitForSeconds(0.2f);
            isShot = false;
            float rvx = Random.Range(1f, 4f);
            float rvy = Random.Range(1f, 4f);
            rb.velocity = new Vector2(rvx, rvy);
            Debug.Log(isShot);
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

    IEnumerator ShotTravel(Vector2 dir, float shootForce)
    {
        isShot = true;
        Debug.Log(isShot);
        rb.velocity = Vector2.zero;

        while((Vector2)transform.position != dir)
        {
            transform.position = Vector2.MoveTowards(transform.position, dir, shootForce * Time.deltaTime);
        }

        //yield return new WaitForSeconds(0.2f);
        isShot = false;
        float rvx = Random.Range(1f, 4f);
        float rvy = Random.Range(1f, 4f);
        rb.velocity = new Vector2(rvx, rvy);
        Debug.Log(isShot);

        yield return null;
    }

    //void OnDrawGizmos()
    //{
    //    if(initialized)
    //    {
    //        Gizmos.DrawWireSphere(transform.position, bfactory.GetRange());
    //    }
    //}

}
