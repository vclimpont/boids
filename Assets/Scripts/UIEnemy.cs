using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemy : MonoBehaviour
{
    [SerializeField] Camera mainCamera = null;
    [SerializeField] GameObject UIEnemyHealth = null;

    public void AddUIToEnemy(EnemyController enemy)
    {
        GameObject go = Instantiate(UIEnemyHealth);
        go.transform.SetParent(transform);

        UIEnemyHealthCheck UIEnemyHC = go.GetComponent<UIEnemyHealthCheck>();
        UIEnemyHC.Initialize(enemy, mainCamera);
    }
}
