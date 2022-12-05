using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoiderSpawner : MonoBehaviour
{
    public GameObject prefab;
    public List<Transform> spawnPoints;
    float lastSpawnTime = 0;
    Population population;
    
    // Start is called before the first frame update
    void Start()
    {
        
        population = new Population(prefab, spawnPoints);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnTime > 5)
        {
            lastSpawnTime = Time.time;
            population.Respawn();

        }

    }
}
