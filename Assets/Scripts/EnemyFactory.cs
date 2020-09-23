using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] UIEnemy canvas = null;
    [SerializeField] GameObject enemyPrefab = null;

    [Header("Enemy settings")]
    [SerializeField] private int health = 0;
    [SerializeField] private float minSpeed = 0f;
    [SerializeField] private float maxSpeed = 0f;
    [SerializeField] private float range = 0f;
    [SerializeField] private float fearForce = 0f;
    [SerializeField] private float spawnRate = 0f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while(true)
        {
            GameObject go = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            EnemyController enemy = go.GetComponent<EnemyController>();

            float rvx = Random.Range(-1f, 1f);
            float rvy = Random.Range(-1f, 1f);
            float rs = Random.Range(minSpeed, maxSpeed);
            Vector2 dir = new Vector2(rvx, rvy);

            enemy.Initialize(health, rs, range, fearForce, dir);

            canvas.AddUIToEnemy(enemy);

            yield return new WaitForSeconds(spawnRate);
        }
    }

}
