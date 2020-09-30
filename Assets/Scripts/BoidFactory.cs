using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFactory : MonoBehaviour
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


    // Start is called before the first frame update
    void Start()
    {
        GameManager.AddBoids(numberOfBoids);
        GenerateBoids();
    }

    void GenerateBoids()
    {
        for(int i = 0; i < numberOfBoids; i++)
        {
            float rpx = Random.Range(-boundX, boundX);    // position x
            float rpy = Random.Range(-boundY, boundY);    // position y
            InstantiateBoid(new Vector2(transform.position.x + rpx, transform.position.y + rpy));
        }
    }

    public void InstantiateBoid(Vector2 position)
    {
        Boid boid = Instantiate(boidPrefab, Vector2.zero, Quaternion.identity).GetComponent<Boid>(); // Instantiate a new boid

        float rvx = Random.Range(-1f, 1f);      // direction x
        float rvy = Random.Range(-1f, 1f);      // direction y 
        float rs = Random.Range(1f, 4f);        // speed

        boid.Initialize(rs, position, new Vector2(rvx, rvy), this); // Set random position and random velocity
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
}
