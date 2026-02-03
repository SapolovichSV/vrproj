using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;
    
    [Header("Scoring")]
    public int score = 0;

    void Awake()
    {
        // Simple singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start spawning zombies
        if (zombiePrefab != null && spawnPoints.Length > 0)
        {
            StartCoroutine(SpawnZombies());
        }
        else
        {
            Debug.LogError("GameManager: Zombie prefab or spawn points not set up!");
        }
    }

    IEnumerator SpawnZombies()
    {
        while (true) // Infinite loop to keep spawning
        {
            yield return new WaitForSeconds(spawnInterval);

            // Pick a random spawn point
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnPointIndex];

            // Create a new zombie
            Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score); // For now, we'll log the score. UI can be added later.
    }
}
