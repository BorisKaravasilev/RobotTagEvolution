using System.Collections.Generic;
using UnityEngine;

public class AvoiderSpawner : MonoBehaviour
{
    public float GenerationLifespan = 10f; 
    public GameObject prefab;
    float lastSpawnTime = 0;
    Population population;
    public double mutationProbability = 0.0;
    public double maxMutationRate = 0.0;
    private List<Transform> spawnPoints = new();

    // Start is called before the first frame update

    void Start()
    {
        var respawnGOs = GameObject.FindGameObjectsWithTag("Respawn");

        foreach (var gameObject in respawnGOs)
        {
            spawnPoints.Add(gameObject.transform);
        }
        
        population = new Population(prefab, spawnPoints, mutationProbability, maxMutationRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnTime > GenerationLifespan)
        {
            lastSpawnTime = Time.time;
            population.Respawn();
        }
    }
}
