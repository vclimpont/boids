﻿using System.Collections;
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
            Debug.DrawLine(transform.position, (Vector2)transform.position + (rb.velocity.normalized * 0.5f), Color.white);
            CheckForObstacles();

            LimitVelocity();

            ApplyRules();
            BoundVelocity();
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, bfactory.GetRange() * 1.5f, LayerMask.GetMask("Obstacle"));

        if(hit.collider != null)        // If it hits an obstacle
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, Time.deltaTime);
            AvoidObstacle();
        }
    }

    void AvoidObstacle()
    {
        float theta_p =  Mathf.PI / 6f;
        float theta_m = -Mathf.PI / 6f;

        for (int i = 0; i <= 6; i++)
        {
            Vector2 dir = rb.velocity.normalized;
            Vector2 dirToCast = Vector2.zero;

            dirToCast = CastRay(theta_p * i, dir);
            if (dirToCast != Vector2.zero)
            {
                Debug.DrawLine(transform.position, (Vector2)transform.position + (dirToCast * bfactory.GetRange() * 2f), Color.green, Time.deltaTime);
                rb.velocity = (dirToCast * rb.velocity.magnitude * 0.5f) + (rb.velocity * 0.3f);
                break;
            }

            dirToCast = CastRay(theta_m * i, dir);
            if (dirToCast != Vector2.zero)
            {
                Debug.DrawLine(transform.position, (Vector2)transform.position + (dirToCast * bfactory.GetRange() * 2f), Color.green, Time.deltaTime);
                rb.velocity = (dirToCast * rb.velocity.magnitude * 0.5f) + (rb.velocity * 0.3f);
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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToCast, bfactory.GetRange() * 1.5f, LayerMask.GetMask("Obstacle"));

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

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    //void OnDrawGizmos()
    //{
    //    if(initialized)
    //    {
    //        Gizmos.DrawWireSphere(transform.position, bfactory.GetRange());
    //    }
    //}

}
