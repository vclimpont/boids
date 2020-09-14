using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] GameObject boidPrefab = null;
    [SerializeField] private int numberOfBoids = 0;

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoids();
    }

    void GenerateBoids()
    {
        for(int i = 0; i < numberOfBoids; i++)
        {
            Boid boid = Instantiate(boidPrefab, Vector2.zero, Quaternion.identity).GetComponent<Boid>();

            float rpx = Random.Range(-10f, 10f);
            float rpy = Random.Range(-10f, 10f);
            float rvx = Random.Range(-1f, 1f);
            float rvy = Random.Range(-1f, 1f);
            float rs = Random.Range(0f, 4f);

            boid.Initialize(rs, new Vector2(rpx, rpy), new Vector2(rvx, rvy));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
