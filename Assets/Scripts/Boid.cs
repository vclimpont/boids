using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    [SerializeField] private float speed;

    private Rigidbody2D rb;

    public void Initialize(float speed, Vector2 pos, Vector2 dir)
    {
        rb = GetComponent<Rigidbody2D>();

        transform.position = pos;
        rb.velocity = dir * speed;

    }
}
