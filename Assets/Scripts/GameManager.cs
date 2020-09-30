using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static int boidsLeft = 0;
    private static int score = 0;

    private void Start()
    {
        boidsLeft = 0;
        score = 0;
    }

    void Update()
    {
        if(boidsLeft <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public static void AddBoids(int nbBoids)
    {
        boidsLeft += nbBoids;
        Debug.Log(boidsLeft);
    }

    public static void RemoveBoids(int nbBoids)
    {
        boidsLeft -= nbBoids;
        Debug.Log(boidsLeft);
    }

    public static void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }

    public static int GetScore()
    {
        return score;
    }
}
