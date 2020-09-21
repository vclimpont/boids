﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float range = 0;


    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveDirection;
    private int nbBoids;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        nbBoids = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputsMovement();

        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("SHROOM");
        }
    }

    void FixedUpdate()
    {
        Move();
        FlipSprite();
    }

    void CheckInputsMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    void FlipSprite()
    {
        sr.flipX = (rb.velocity.x < 0);
    }
}