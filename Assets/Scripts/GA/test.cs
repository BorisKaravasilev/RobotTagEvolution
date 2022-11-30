using System;
using System.Collections;
using UnityEngine;


public class test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}


public class GeneticAlgorythm : MonoBehaviour
{
    Population population = new Population();
    Individual fittest;
    int generationCount;
    int popSize = 10;


    void Update()
    {
        System.Random random = new System.Random();
        GeneticAlgorythm ga = new GeneticAlgorythm();

        ga.population.initPop(popSize);
        ga.population.calculateFitness();

        Console.WriteLine("Generation: " + ga.generationCount + "Fittest: " + ga.population.getFittestIndex());


    }

}