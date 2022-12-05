using System.Collections;
using UnityEngine;
using System;
using JetBrains.Annotations;
using System.IO.IsolatedStorage;
using static Unity.VisualScripting.LudiqRootObjectEditor;
using Unity.VisualScripting;
using System.Collections.Generic;


    public class Population


    {
        int popSize = 4;
        int fittestIndex = 0;
        Individual[] individuals;
        //Individual[] rouletteList;
        List<Individual> rouletteList = new List<Individual>();
        GameObject individualType;
        List<Transform> spawnPoints;
        Individual specimen1;
        Individual specimen2;
        List<Individual> offSprings = new List<Individual>();
        double mutationProbability = 0.1;
        double maxMutationRate = 0.1;
        int generationCount = 0;
        Individual fittest;

    public Population(GameObject prefab, List<Transform> spawnPoints)
    {
        individualType = prefab;
        this.spawnPoints = spawnPoints;
        this.popSize = spawnPoints.Count;
        this.individuals = new Individual[popSize];
        
        for (int i = 0; i < individuals.Length; i++)
        {
            individuals[i] = new Individual(prefab, spawnPoints[i].position);
        }

    }



    public void Respawn() {
        populateRouletteList();
        populateOffSpingsList();
        



    }
        
        
    public Individual getFittest()

    {
        double maxFit = 0;
        
        for( int i = 0; i < individuals.Length; i++ ){
            if (maxFit < individuals[i].getFitness())
            {
                maxFit = individuals[i].getFitness();
                fittestIndex = i;
            }
        }
        
        return individuals[fittestIndex];
    }

    public int getFittestIndex()
    {
        return fittestIndex;
    }


    void populateRouletteList()
    {
        foreach (Individual individual in individuals){
                
            for (int j = 0; j < individual.getFitness()+1; j++)
            {
                rouletteList.Add(individual);

            }

        }
    }

    public List<Individual> getRouletteList()
    {
        return rouletteList;
    }

    public Individual[] getIndividuals()
    {
        return individuals;
    }

    void selection()
    {
        populateRouletteList();
        System.Random random = new System.Random();
        int index1 = random.Next(0, getRouletteList().Count);
        int index2 = random.Next(0, getRouletteList().Count);

        specimen1 = getRouletteList()[index1];
        specimen2 = getRouletteList()[index2];
        fittest = getFittest();
    }
    Individual crossover()
    {
        selection();
        System.Random rn = new System.Random();

        //Select a random crossover point
        int crossOverPoint = rn.Next(getIndividuals()[0].getChromosomeLength());

        //Swap values
        for (int i = 0; i < crossOverPoint; i++)
        {

            specimen1.getChromosome()[i] = specimen2.getChromosome()[i];
            

        }
        offSprings.Add(specimen1);
        return specimen1;


    }


    void mutation()
    {
        System.Random rn = new System.Random();

        foreach (Individual offspring in offSprings)
        {
            double chance = rn.NextDouble();

            if (chance <= mutationProbability)
            {
                //Select a random mutation point for specimen 1
                int mutationPoint = rn.Next(getIndividuals()[0].getChromosomeLength());
                //Define mutation rate
                double mutationRate = (rn.NextDouble() * (2 * maxMutationRate)) - maxMutationRate;
                //Mutate gene
                specimen1.getChromosome()[mutationPoint] += mutationRate;

            }
        }
    }



    void populateOffSpingsList()
    {
        //offSprings.Add(getFittest());
        for (int i = 0; i < popSize; i++) {
            crossover();
            offSprings[i].prefabInstance = individuals[i].prefabInstance;
            offSprings[i].prefabInstance.transform.position = spawnPoints[i].position;
        }
        mutation();
    }

}






