using System.Collections;
using UnityEngine;
using System;



public class Individual
{
    private double[] chromosome;
    public GameObject prefabInstance;

    public Individual(double [] chromosome, GameObject instance)
    {
        prefabInstance = instance;
        SetChromosome(chromosome);
    }

    public Individual(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        prefabInstance = GameObject.Instantiate(prefab);
        prefabInstance.transform.position = spawnPosition;
        prefabInstance.transform.rotation = spawnRotation;
        
        SetChromosome(prefabInstance.GetComponent<Thymio>().GetChromosome());
    }

    public void SetChromosome(double [] chromosome)
    {
        this.chromosome = chromosome;
        prefabInstance.GetComponent<Thymio>().SetChromosome(this.chromosome);
    }

    public double getFitness()
    {
        return prefabInstance.GetComponent<Thymio>().GetFitness();
    }

    public int getChromosomeLength()
    {
        return chromosome.Length;
    }

    public double[] getChromosome()
    {
        return chromosome;
    }




}
