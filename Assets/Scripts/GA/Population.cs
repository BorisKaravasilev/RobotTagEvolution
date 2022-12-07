using System;
using UnityEngine;
using System.Collections.Generic;

public class Population
{
    int popSize = 4;
    int fittestIndex = 0;

    Individual[] individuals;

    //Individual[] rouletteList;
    List<Individual> rouletteList = new List<Individual>();
    List<Transform> spawnPoints;
    List<Individual> offSprings = new List<Individual>();
    double mutationProbability = 0.1;
    double maxMutationRate = 0.1;
    int generationCount = 0;

    public Population(GameObject prefab, List<Transform> spawnPoints, double mutationProbability, double maxMutationRate)
    {
        this.spawnPoints = spawnPoints;
        this.popSize = spawnPoints.Count;
        this.individuals = new Individual[popSize];
        this.mutationProbability = mutationProbability;
        this.maxMutationRate = maxMutationRate;

        for (int i = 0; i < individuals.Length; i++)
        {
            individuals[i] = new Individual(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
            individuals[i].prefabInstance.name = $"Avoider {i}";
        }
    }

    public void Respawn()
    {
        populateOffSpingsList();
    }

    public Individual getFittest()
    {
        double maxFit = 0;

        for (int i = 0; i < individuals.Length; i++)
        {
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
        foreach (Individual individual in individuals)
        {
            for (int j = 0; j < individual.getFitness() + 1; j++)
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

    (Individual, Individual) Selection()
    {
        populateRouletteList();
        
        System.Random random = new System.Random();
        int index1 = random.Next(0, getRouletteList().Count);
        int index2 = random.Next(0, getRouletteList().Count);

        Individual specimen1 = getRouletteList()[index1];
        Individual specimen2 = getRouletteList()[index2];
        // fittest = getFittest();

        return (specimen1, specimen2);
    }

    double[] Crossover()
    {
        (Individual, Individual) specimens = Selection();
        Individual specimen1 = specimens.Item1;
        Individual specimen2 = specimens.Item2;
        
        System.Random rn = new System.Random();

        //Select a random crossover point
        int crossOverPoint = rn.Next(getIndividuals()[0].getChromosomeLength());

        double[] offspringChromosome = new double[specimen1.getChromosomeLength()];

        //Swap values
        for (int i = 0; i < specimen1.getChromosomeLength(); i++)
        {
            if (i < crossOverPoint)
            {
                offspringChromosome[i] = specimen1.getChromosome()[i];
            }
            else
            {
                offspringChromosome[i] = specimen2.getChromosome()[i];
            }
        }
        
        return offspringChromosome;
    }

    void Mutation()
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
                offspring.getChromosome()[mutationPoint] += mutationRate;
            }
        }
    }

    void populateOffSpingsList()
    {
        double totalFitness = 0;
        string fitnessOverview = $"Generation {generationCount}  |  Fitness: ";
        
        foreach (var individual in individuals)
        {
            double fitness = individual.getFitness();
            totalFitness += fitness;
            // fitnessOverview += fitness.ToString() + ", ";
        }

        fitnessOverview += "    Total Fitness:" + totalFitness.ToString();
        Debug.Log(fitnessOverview);
        
        // var fittestOffSpring = getFittest();
        // Individual offSpringToAdd = new Individual(fittestOffSpring.getChromosome(), fittestOffSpring.prefabInstance);
        // offSprings.Add(offSpringToAdd);
        //     
        // offSpringToAdd.prefabInstance.GetComponent<Thymio>().Respawn();
        
        
        for (int i = 0; i < popSize - 1; i++)
        {
            double[] offspringChromosome = Crossover();
            Individual offspring = new Individual(offspringChromosome, individuals[i].prefabInstance);
            offSprings.Add(offspring);
            
            offSprings[i].prefabInstance.GetComponent<Thymio>().Respawn();

            // Debug.Log(offspring.prefabInstance.name + ": " + offspring.prefabInstance.GetComponent<Thymio>().Chromosome[0] + ", " + offspring.prefabInstance.GetComponent<Thymio>().Chromosome[1]);
        }

        Mutation();
        individuals = offSprings.ToArray();
        offSprings.Clear();
        generationCount++;
    }
}
