using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float health;
    private float speed;
    private float range;
    private float fearForce;
    private int boidsEaten;
    private float stunDuration;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;

    private ObstacleChecker obsChecker;
    private BoidFactory bFactory;

    private bool initialized = false;
    private bool isStun = false;

    public void Initialize(float health, float speed, float range, float fearForce, Vector2 dir)
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        this.health = health;
        this.speed = speed;
        this.range = range;
        this.fearForce = fearForce;
        rb.velocity = dir * speed;
        boidsEaten = 0;
        stunDuration = 1.5f;

        obsChecker = new ObstacleChecker(gameObject, range);

        initialized = true;
        isStun = false;
    }

    void FixedUpdate()
    {
        if(initialized && !isStun)
        {
            FlipSprite();
            FollowCloseBoids();
            obsChecker.CheckForObstacles();
        }
    }

    void FollowCloseBoids()
    {
        Collider2D[] closeBoids = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask("Boid"));   // Get all boids in the range

        float size = 0f;
        Vector2 avgPosition = Vector2.zero;

        for (int i = 0; i < closeBoids.Length; i++)         // For every close boid
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if (!boid.CompareState(Boid.State.Shot))
            {
                avgPosition += boid.GetPosition();          // Calculate average position
                size++;
            }
        }

        if (size > 0)
        {
            avgPosition /= size;

            Vector2 targetDirection = avgPosition - (Vector2)transform.position;        // Get the direction from current position to the average position calculated and normalize it
            targetDirection = targetDirection.normalized;

            rb.velocity = targetDirection * speed;   // Add an amount of 
        }
    }

    void EatBoid(Boid boid)
    {
        if(bFactory == null)
        {
            bFactory = boid.GetBoidFactory();
        }

        Destroy(boid.gameObject);
        UpgradeStats();
        boidsEaten++;
    }

    void SpitBoids()
    {
        for (int i = 0; i < boidsEaten; i++)
        {
            float rx = Random.Range(-1f, 1f); 
            float ry = Random.Range(-1f, 1f);
            Vector2 boidPosition = new Vector2(transform.position.x + rx, transform.position.y + ry);
            bFactory.InstantiateBoid(boidPosition);
        }
    }

    void Damage()
    {
        health--;
        if(health <= 0)
        {
            Die();
        }
        else
        {
            StopCoroutine("GetStunned");
            StartCoroutine("GetStunned");
        }
    }

    void Die()
    {
        SpitBoids();
        Destroy(gameObject);
    }

    void UpgradeStats()
    {
        if (transform.localScale.x < 5)
        {
            transform.localScale += new Vector3(0.25f, 0.25f, 0);
        }

        health++;
    }

    void FlipSprite()
    {
        sr.flipX = rb.velocity.x < 0;
    }

    void Stun()
    {
        isStun = true;
        animator.SetBool("isStunned", true);

        rb.velocity = Vector2.zero;
    }

    void WakeUp()
    {
        isStun = false;
        animator.SetBool("isStunned", false);

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        rb.velocity = new Vector2(x, y) * speed;
    }

    public float GetRange()
    {
        return range;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public float GetFearForce()
    {
        return fearForce;
    }

    IEnumerator GetStunned()
    {
        Stun();
        yield return new WaitForSeconds(stunDuration);
        WakeUp();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Boid"))
        {
            Boid boid = collision.gameObject.GetComponent<Boid>();

            if (boid.CompareState(Boid.State.Shot))
            {
                Damage();
            }
            else if (!isStun)
            {
                EatBoid(boid);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (initialized)
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
