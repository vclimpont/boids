using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFactory : MonoBehaviour
{
    [SerializeField] GameObject boidPrefab = null;
    [SerializeField] private int numberOfBoids = 0;
    [SerializeField] private float range = 0;
    [SerializeField] private float maxVelocity = 0;
    [SerializeField] private float velLimitFactor = 0;
    [SerializeField] private float boundX = 0;
    [SerializeField] private float boundY = 0;

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoids();
    }

    void GenerateBoids()
    {
        for(int i = 0; i < numberOfBoids; i++)
        {
            Boid boid = Instantiate(boidPrefab, Vector2.zero, Quaternion.identity).GetComponent<Boid>(); // Instantiate a new boid

            float rpx = Random.Range(-10f, 10f);
            float rpy = Random.Range(-10f, 10f);
            float rvx = Random.Range(-1f, 1f);
            float rvy = Random.Range(-1f, 1f);
            float rs = Random.Range(0f, 4f);

            boid.Initialize(rs, new Vector2(rpx, rpy), new Vector2(rvx, rvy), this); // Set random position and random velocity
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

    public float GetBoundX()
    {
        return boundX;
    }

    public float GetBoundY()
    {
        return boundY;
    }
}
