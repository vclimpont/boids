using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleChecker
{

    private GameObject go;
    private Rigidbody2D rb;
    private float range;

    public ObstacleChecker(GameObject _go, float _range)
    {
        go = _go;
        range = _range;

        rb = go.GetComponent<Rigidbody2D>();
    }


    public void CheckForObstacles()
    {
        RaycastHit2D hit = Physics2D.Raycast(go.transform.position, rb.velocity.normalized, range, LayerMask.GetMask("Obstacle"));

        if (hit.collider != null)        // If it hits an obstacle
        {
            Debug.DrawLine(go.transform.position, hit.point, Color.red, Time.deltaTime);
            AvoidObstacle();
        }
    }

    void AvoidObstacle()
    {
        float theta_p = Mathf.PI / 8f;
        float theta_m = -Mathf.PI / 8f;

        for (int i = 0; i <= 8; i++)
        {
            Vector2 dir = rb.velocity.normalized;
            Vector2 dirToCast;

            dirToCast = CastRay(theta_p * i, dir);
            if (dirToCast != Vector2.zero)
            {
                Debug.DrawLine(go.transform.position, (Vector2)go.transform.position + (dirToCast * range), Color.green, Time.deltaTime);
                rb.velocity = dirToCast * rb.velocity.magnitude;
                break;
            }

            dirToCast = CastRay(theta_m * i, dir);
            if (dirToCast != Vector2.zero)
            {
                Debug.DrawLine(go.transform.position, (Vector2)go.transform.position + (dirToCast * range), Color.green, Time.deltaTime);
                rb.velocity = dirToCast * rb.velocity.magnitude;
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

        RaycastHit2D hit = Physics2D.Raycast(go.transform.position, dirToCast, range, LayerMask.GetMask("Obstacle"));

        if (hit.collider == null)        // If it does not hit an obstacle
        {
            return dirToCast;
        }
        else
        {
            Debug.DrawLine(go.transform.position, hit.point, Color.red, Time.deltaTime);
            return Vector2.zero;
        }
    }
}
