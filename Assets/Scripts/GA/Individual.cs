using System.Collections;
using UnityEngine;
using System;



public class Individual
{
    int fitness = 0;
    double [] chromosome = new double[14];
    int chromosomeLength = 14;

    public Individual()
    {
        System.Random rnd = new System.Random();

        for(int j = 0; j < chromosomeLength; j++)
        {
            chromosome[j] = rnd.NextDouble();
        }

        fitness = 0;
    }

    //Calc fitness

    public void calcFitness()
    {

        // TODO - it has to come from unity Thymio.getFittness
            


    }

    public int getFitness()
    {
        return fitness;
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
