using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoidFactory bfactory;

    private float speed;
    private bool initialized = false;

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


            MoveWith();
            BoundVelocity();
        }
    }

    void MoveWith()
    {
        Collider2D[] closeBoids = Physics2D.OverlapCircleAll(transform.position, bfactory.GetRange());
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
            rb.velocity += (avgVelocity * (1f/8f));
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


    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, bfactory.GetRange());
    //}

}
