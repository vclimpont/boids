using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthCheck : MonoBehaviour
{
    private EnemyController enemy;
    private Text textLabel;
    private Camera mainCamera;
    private Vector2 offset;
    private int healthLimit;

    private bool initialized = false;

    // Update is called once per frame
    void Update()
    {
        if(initialized)
        {
            if(enemy.GetHealth() == 1)
            {
                Destroy(gameObject);
            }
            transform.position = mainCamera.WorldToScreenPoint(enemy.GetPosition() + offset);
            SetTextWith(enemy.GetHealth());
        }
    }

    //faire ça en fonction du scaling
    void SetTextWith(int enemyHealth)
    {
        textLabel.text = "" + enemyHealth;

        if(enemyHealth > 10 && enemyHealth < healthLimit)
        {
            textLabel.fontSize = 18 + (enemyHealth - 10);
            offset = new Vector2(-0.5f, 1f) * (2f / (healthLimit - enemyHealth));

            float f = (5f - (healthLimit - enemyHealth)) / 4f;
            textLabel.color = Color.Lerp(Color.green, Color.red, f);
        }
    }

    public void Initialize(EnemyController enemy, Camera mainCamera)
    {
        textLabel = GetComponent<Text>();
        offset = new Vector2(-0.5f, 1f);
        healthLimit = 16;
        this.enemy = enemy;
        this.mainCamera = mainCamera;

        initialized = true;
    }
}
