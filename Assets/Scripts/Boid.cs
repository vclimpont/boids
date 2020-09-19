using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoidFactory bfactory;

    private float speed;

    private bool initialized = false;
    private int boidLayer;


    public void Initialize(float speed, Vector2 pos, Vector2 dir, BoidFactory bfactory)
    {
        rb = GetComponent<Rigidbody2D>();
        this.bfactory = bfactory;
        this.speed = speed;

        transform.position = pos;
        rb.velocity = dir * speed;

        initialized = true;
    }

    void FixedUpdate()
    {
        if(initialized)
        {
            LimitVelocity();


            ApplyRules();
            BoundVelocity();
        }
    }

    void ApplyRules()
    {
        boidLayer = 1 << LayerMask.NameToLayer("Boid"); 
        Collider2D[] closeBoids = Physics2D.OverlapCircleAll(transform.position, bfactory.GetRange(), boidLayer);

        MoveWith(closeBoids);
        MoveTowardsCenter(closeBoids);
        AvoidObstacles(closeBoids);
    }

    void MoveWith(Collider2D[] closeBoids)
    {
        float size = 0f;
        Vector2 avgVelocity = Vector2.zero;

        for(int i = 0; i < closeBoids.Length; i++)
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if(boid != this)
            {
                avgVelocity += boid.GetVelocity();
                size++;
            }
        }

        if(size > 0)
        {
            avgVelocity /= size;
            rb.velocity += (avgVelocity * (bfactory.GetAlignmentFactor() / 10.0f));
        }
    }

    void MoveTowardsCenter(Collider2D[] closeBoids)
    {
        float size = 0f;
        Vector2 avgPosition = Vector2.zero;

        for (int i = 0; i < closeBoids.Length; i++)
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if (boid != this)
            {
                avgPosition += boid.GetPosition();
                size++;
            }
        }

        if (size > 0)
        {
            avgPosition /= size;

            Vector2 targetPosition = avgPosition - (Vector2)transform.position;
            targetPosition = targetPosition.normalized;

            rb.velocity += (targetPosition * speed * (bfactory.GetCohesionFactor() / 10.0f));
        }
    }

    void AvoidObstacles(Collider2D[] closeBoids)
    {
        float size = 0f;
        Vector2 avgPosition = Vector2.zero;

        for (int i = 0; i < closeBoids.Length; i++)
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if (boid != this)
            {
                avgPosition += boid.GetPosition();
                size++;
            }
        }

        if (size > 0)
        {
            avgPosition /= size;

            Vector2 targetPosition = (Vector2)transform.position - avgPosition;
            targetPosition = targetPosition.normalized;

            rb.velocity += (targetPosition * speed * (bfactory.GetSeparationFactor() / 10.0f));
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

        if(transform.position.x <= -boundX)
        {
            rb.velocity = new Vector2(2f, rb.velocity.y);
        }
        else if(transform.position.x >= boundX)
        {
            rb.velocity = new Vector2(-2f, rb.velocity.y);
        }

        if (transform.position.y <= -boundY)
        {
            rb.velocity = new Vector2(rb.velocity.x, 2f);
        }
        else if (transform.position.y >= boundY)
        {
            rb.velocity = new Vector2(rb.velocity.x, -2f);
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

    void OnDrawGizmos()
    {
        if(initialized)
        {
            Gizmos.DrawWireSphere(transform.position, bfactory.GetRange());
        }
    }

}
