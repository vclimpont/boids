using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Rigidbody2D rb;

    private float speed;
    private float range;
    private bool initialized = false;

    public void Initialize(float speed, Vector2 pos, Vector2 dir, float range)
    {
        rb = GetComponent<Rigidbody2D>();
        this.speed = speed;
        this.range = range;

        transform.position = pos;
        rb.velocity = dir * speed;

        initialized = true;
    }

    void FixedUpdate()
    {
        if(initialized)
        {
            MoveWith();
        }
    }

    void MoveWith()
    {
        Collider2D[] closeBoids = Physics2D.OverlapCircleAll(transform.position, range);

        Vector2 avgDirection = Vector2.zero;

        for(int i = 0; i < closeBoids.Length; i++)
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            if(boid != this)
            {
                Debug.Log("hello toi");
                avgDirection += boid.GetVelocity();
            }
            else
            {
                Debug.Log("c'est moi");
            }
        }

        if(closeBoids.Length > 0)
        {
            avgDirection /= closeBoids.Length;
            rb.velocity = avgDirection * speed;
        }
    }

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
