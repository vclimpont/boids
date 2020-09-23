using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float health = 0f;
    [SerializeField] private float speed = 0f; 
    [SerializeField] private float range = 0f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private ObstacleChecker obsChecker;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        obsChecker = new ObstacleChecker(this.gameObject, range);
    }

    void FixedUpdate()
    {
        FlipSprite();
    }
    
    void FlipSprite()
    {
        sr.flipX = rb.velocity.x < 0;
    }
}
