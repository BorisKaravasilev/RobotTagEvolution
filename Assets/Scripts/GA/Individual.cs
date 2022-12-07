using System.Collections;
using UnityEngine;
using System;



public class Individual
{
    private double[] chromosome;
    int chromosomeLength = 2;
    public GameObject prefabInstance;

    public Individual(double [] chromosome, GameObject instance)
    {
        prefabInstance = instance;
        SetChromosome(chromosome);
    }

    public Individual(GameObject prefab, Vector3 spawnPos)
    {
        double[] randomChromosome = new double[chromosomeLength];
        prefabInstance = GameObject.Instantiate(prefab);
        prefabInstance.transform.position = spawnPos;


        System.Random rnd = new System.Random();

        for(int j = 0; j < chromosomeLength; j++)
        {
            randomChromosome[j] = 2.0 * rnd.NextDouble() - 1.0;
        }
        
        SetChromosome(randomChromosome);
    }

    public void SetChromosome(double [] chromosome)
    {
        this.chromosome = chromosome;
        prefabInstance.GetComponent<Thymio>().Chromosome = this.chromosome;
    }

    public double getFitness()
    {
        return prefabInstance.GetComponent<Thymio>().GetFitness();
    }

    public int getChromosomeLength()
    {
        return chromosomeLength;
    }

    public double[] getChromosome()
    {
        return chromosome;
    }




}
