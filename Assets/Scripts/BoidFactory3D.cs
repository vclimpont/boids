﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFactory3D : MonoBehaviour
{
    [SerializeField] GameObject boidPrefab = null;
    [SerializeField] private int numberOfBoids = 0;
    [SerializeField] private float range = 0;

    [Header("Velocity Settings")]
    [Range(0.0f, 10.0f)]
    [SerializeField] private float maxVelocity = 0;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float velLimitFactor = 0;

    [Header("Rules Settings")]
    [Range(0.0f, 10f)]
    [SerializeField] private float separationFactor = 0;
    [Range(0.0f, 10f)]
    [SerializeField] private float alignmentFactor = 0;
    [Range(0.0f, 10f)]
    [SerializeField] private float cohesionFactor = 0;

    [Header("Scene Limits")]
    [SerializeField] private float boundX = 0;
    [SerializeField] private float boundY = 0;
    [SerializeField] private float boundZ = 0;


    // Start is called before the first frame update
    void Start()
    {
        GenerateBoids();
    }

    void GenerateBoids()
    {
        for (int i = 0; i < numberOfBoids; i++)
        {
            Boid3D boid = Instantiate(boidPrefab, Vector3.zero, Quaternion.identity).GetComponent<Boid3D>(); // Instantiate a new boid

            float rpx = Random.Range(-10f, 10f);    // position x
            float rpy = Random.Range(-10f, 10f);    // position y
            float rpz = Random.Range(-10f, 10f);    // position z
            float rvx = Random.Range(-1f, 1f);      // direction x
            float rvy = Random.Range(-1f, 1f);      // direction y
            float rvz = Random.Range(-1f, 1f);      // direction z
            float rs = Random.Range(1f, 4f);        // speed

            boid.Initialize(rs, new Vector3(rpx, rpy, rpz), new Vector3(rvx, rvy, rvz), this); // Set random position and random velocity
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetRange()
    {
        return range;
    }

    public float GetMaxVelocity()
    {
        return maxVelocity;
    }

    public float GetVelLimitFactor()
    {
        return velLimitFactor;
    }

    public float GetSeparationFactor()
    {
        return separationFactor;
    }

    public float GetAlignmentFactor()
    {
        return alignmentFactor;
    }

    public float GetCohesionFactor()
    {
        return cohesionFactor;
    }

    public float GetBoundX()
    {
        return boundX;
    }

    public float GetBoundY()
    {
        return boundY;
    }

    public float GetBoundZ()
    {
        return boundZ;
    }
}
