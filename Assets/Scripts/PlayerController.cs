using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float range = 0;
    [SerializeField] private float attractForce = 0;
    [SerializeField] private float shootForce = 0;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveDirection;
    private int nbBoids;
    private bool hasShot;
    private int boidLayer;
    private Collider2D[] closeBoids;
    private Vector2 shotPosition;

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
            SetShotPosition();
        }
        else
        {
            hasShot = false;
        }
    }

    void SetShotPosition()
    {
        Vector2 mp = Input.mousePosition;
        mp = Camera.main.ScreenToWorldPoint(mp);
        shotPosition = mp;
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
        closeBoids = Physics2D.OverlapCircleAll(transform.position, range, boidLayer);   // Get all boids in the range
        attracting = true;

        nbBoids = closeBoids.Length;
        for(int i = 0; i < nbBoids; i++)
        {
            Boid boid = closeBoids[i].GetComponent<Boid>();
            boid.StartFollowing(this);
        }
    }

    void ShotBoid()
    {
        Boid boid = closeBoids[0].GetComponent<Boid>();

        Vector2 dir = shotPosition - (Vector2)transform.position;
        dir = dir.normalized;
        boid.ShotAt(transform.position, dir, shootForce);
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void FlipSprite()
    {
        sr.flipX = (rb.velocity.x < 0);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public float GetAttractForce()
    {
        return attractForce;
    }

    public float GetRange()
    {
        return range;
    }

    void OnDrawGizmos()
    {
        if(attracting)
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Enemy"))
        {
            Die();
        }
    }
}
