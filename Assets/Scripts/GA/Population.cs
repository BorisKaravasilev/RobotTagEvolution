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
        int popSize = 10;
        int fittestIndex = 0;
        Individual[] individuals;
        //Individual[] rouletteList;
        List<Individual> rouletteList = new List<Individual>();
        

        public void initPop(int popSize)
                    
        {
            this.popSize = popSize;
            this.individuals = new Individual[popSize];
            for (int i = 0; i < individuals.Length; i++)
            {
                individuals[i] = new Individual();
            }
        }

        //Get the fittest individual within the population

    public Individual getFittest()

    {
        int maxFit = 0;
            for( int i = 0; i < individuals.Length; i ++ )
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

    //Calculate fitness of each individual
    public void calculateFitness()
    {

        for (int i = 0; i < individuals.Length; i++)
        {
            individuals[i].calcFitness();
        }
        getFittest();
    }

    void populateRouletteList()
    {
        foreach (Individual individual in individuals){
                
            for (int j = 0; j < individual.getFitness(); j++)
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





}
