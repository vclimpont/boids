using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float range = 0;
    [SerializeField] private float attractForce = 0;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveDirection;
    private int nbBoids;
    private bool hasShot;
    private int boidLayer;

    private bool attracting;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        nbBoids = 0;
        hasShot = false;
        attracting = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputsMovement();
        CheckInputShot();

        if (hasShot && CanShot())
        {
            ShotBoid();
        }
    }

    void FixedUpdate()
    {
        Move();
        FlipSprite();

        AttractBoids();
        Debug.Log(nbBoids);
    }

    void CheckInputsMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void CheckInputShot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hasShot = true;
        }
        else
        {
            hasShot = false;
        }
    }

    bool CanShot()
    {
        return nbBoids > 0;
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    void AttractBoids()
    {
        boidLayer = 1 << LayerMask.NameToLayer("Boid");
        Collider2D[] closeBoids = Physics2D.OverlapCircleAll(transform.position, range, boidLayer);   // Get all boids in the range
        attracting = true;

        nbBoids = closeBoids.Length;
        for(int i = 0; i < nbBoids; i++)
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            boid.MoveTowardsPlayer(transform.position, attractForce);
        }
    }

    void ShotBoid()
    {
        Debug.Log("SHROOM");
    }

    void FlipSprite()
    {
        sr.flipX = (rb.velocity.x < 0);
    }

    void OnDrawGizmos()
    {
        if(attracting)
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
