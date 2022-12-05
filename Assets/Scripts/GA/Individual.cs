using System.Collections;
using UnityEngine;
using System;



public class Individual
{
    int fitness = 0;
    double [] chromosome = new double[14];
    int chromosomeLength = 14;
    public GameObject prefabInstance;



    public Individual(GameObject prefab, Vector3 spawnPos)
    {
        this.prefabInstance = GameObject.Instantiate(prefab);
        this.prefabInstance.transform.position = spawnPos;


        System.Random rnd = new System.Random();

        for(int j = 0; j < chromosomeLength; j++)
        {
            chromosome[j] = rnd.NextDouble();
        }



        fitness = 0;
    }
    
    public double getFitness()
    {
        return prefabInstance.GetComponent<ThymioAvoider>().GetFitness();
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
