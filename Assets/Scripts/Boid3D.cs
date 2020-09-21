using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid3D : MonoBehaviour
{
    private Rigidbody rb;
    private BoidFactory3D bfactory;

    private float speed;

    private bool initialized = false;
    private int boidLayer;


    public void Initialize(float speed, Vector3 pos, Vector3 dir, BoidFactory3D bfactory)
    {
        rb = GetComponent<Rigidbody>();
        this.bfactory = bfactory;
        this.speed = speed;

        transform.position = pos;
        rb.velocity = dir * speed;

        initialized = true;
    }

    void FixedUpdate()
    {
        if (initialized)
        {
            Debug.DrawLine(transform.position, transform.position + (rb.velocity.normalized), Color.white);
            CheckForObstacles();

            LimitVelocity();

            ApplyRules();
            BoundVelocity();
        }
    }

    void ApplyRules()
    {
        boidLayer = 1 << LayerMask.NameToLayer("Boid");
        Collider[] closeBoids = Physics.OverlapSphere(transform.position, bfactory.GetRange(), boidLayer);   // Get all boids in the range

        MoveWith(closeBoids);
        MoveTowardsCenter(closeBoids);
        MoveAway(closeBoids);
    }

    void MoveWith(Collider[] closeBoids)
    {
        float size = 0f;
        Vector3 avgVelocity = Vector3.zero;

        for (int i = 0; i < closeBoids.Length; i++)          // For every close boid
        {
            Boid3D boid = closeBoids[i].GetComponent<Boid3D>();
            if (boid != this)
            {
                avgVelocity += boid.GetVelocity();          // Calculate average velocity
                size++;
            }
        }

        if (size > 0)
        {
            avgVelocity /= size;
            rb.velocity += (avgVelocity * (bfactory.GetAlignmentFactor() / 10.0f));     // Add an amount of average velocity calculated to the current velocity 
        }
    }

    void MoveTowardsCenter(Collider[] closeBoids)
    {
        float size = 0f;
        Vector3 avgPosition = Vector3.zero;

        for (int i = 0; i < closeBoids.Length; i++)         // For every close boid
        {
            Boid3D boid = closeBoids[i].GetComponent<Boid3D>();
            if (boid != this)
            {
                avgPosition += boid.GetPosition();          // Calculate average position
                size++;
            }
        }

        if (size > 0)
        {
            avgPosition /= size;

            Vector3 targetPosition = avgPosition - transform.position;        // Get the direction from current position to the average position calculated and normalize it
            targetPosition = targetPosition.normalized;

            rb.velocity += targetPosition * (bfactory.GetCohesionFactor() / 10.0f);   // Add an amount of 
        }
    }

    void MoveAway(Collider[] closeBoids)
    {
        float theta = Mathf.Deg2Rad * 45f;

        float cs = Mathf.Cos(theta);
        float sn = Mathf.Sin(theta);

        for (int i = 0; i < closeBoids.Length; i++)
        {
            Boid3D boid = closeBoids[i].GetComponent<Boid3D>();
            if (boid != this)
            {
                Vector3 targetPosition = transform.position - boid.GetPosition();
                targetPosition = targetPosition.normalized;

                float px = targetPosition.x * cs - targetPosition.y * sn;
                float py = targetPosition.x * sn + targetPosition.y * cs;

                Vector3 dir = new Vector3(px, py, 0);
                rb.velocity += dir * (bfactory.GetSeparationFactor() / 10.0f);
            }
        }
    }

    void CheckForObstacles()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, rb.velocity.normalized, out hit, bfactory.GetRange() * 1.5f, LayerMask.GetMask("Obstacle"));
    
        if (hit.collider != null)        // If it hits an obstacle
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, Time.deltaTime);
            AvoidObstacle();
        }
    }

    void AvoidObstacle()
    {
        //float theta_p = Mathf.PI / 15f;
        //float theta_m = -Mathf.PI / 15f;

        //for (int i = 0; i <= 15; i++)
        //{
        //    Vector2 dir = rb.velocity.normalized;
        //    Vector2 dirToCast;

        //    dirToCast = CastRay(theta_p * i, dir);
        //    if (dirToCast != Vector2.zero)
        //    {
        //        Debug.DrawLine(transform.position, (Vector2)transform.position + (dirToCast * bfactory.GetRange() * 2f), Color.green, Time.deltaTime);
        //        rb.velocity = (dirToCast * rb.velocity.magnitude * 0.5f) + (rb.velocity * 0.3f);
        //        break;
        //    }

        //    dirToCast = CastRay(theta_m * i, dir);
        //    if (dirToCast != Vector2.zero)
        //    {
        //        Debug.DrawLine(transform.position, (Vector2)transform.position + (dirToCast * bfactory.GetRange() * 2f), Color.green, Time.deltaTime);
        //        rb.velocity = (dirToCast * rb.velocity.magnitude * 0.5f) + (rb.velocity * 0.3f);
        //        break;
        //    }
        //}
    }


    Vector3 CastRay(float theta_i, Vector3 dir)
    {
        float cs = Mathf.Cos(theta_i);
        float sn = Mathf.Sin(theta_i);

        float px = dir.x * cs - dir.y * sn;
        float py = dir.x * sn + dir.y * cs;
        Vector3 dirToCast = new Vector3(px, py, 0);

        RaycastHit hit;
        Physics.Raycast(transform.position, dirToCast, out hit, bfactory.GetRange() * 1.5f, LayerMask.GetMask("Obstacle"));

        if (hit.collider == null)        // If it does not hit an obstacle
        {
            return dirToCast;
        }
        else
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, Time.deltaTime);
            return Vector3.zero;
        }
    }

    void LimitVelocity()
    {
        if (rb.velocity.magnitude > bfactory.GetMaxVelocity())
        {
            rb.velocity *= bfactory.GetVelLimitFactor();
        }
    }

    void BoundVelocity()
    {
        float boundX = bfactory.GetBoundX();
        float boundY = bfactory.GetBoundY();
        float boundZ = bfactory.GetBoundZ();

        if (transform.position.x < -boundX)
        {
            transform.position = new Vector3(boundX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > boundX)
        {
            transform.position = new Vector3(-boundX, transform.position.y, transform.position.z);
        }

        if (transform.position.y < -boundY)
        {
            transform.position = new Vector3(transform.position.x, boundY, transform.position.z);
        }
        else if (transform.position.y > boundY)
        {
            transform.position = new Vector3(transform.position.x, -boundY, transform.position.z);
        }

        if (transform.position.z < -boundZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, boundZ);
        }
        else if (transform.position.z > boundZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -boundZ);
        }
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public Vector3 GetPosition()
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
