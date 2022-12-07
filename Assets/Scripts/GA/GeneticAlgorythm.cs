//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets.Scripts.GA
//{
//    public class GeneticAlgorythm : MonoBehaviour
//    {
//        Population population;
//        GeneticAlgorythm ga;
//        Individual fittest;
//        int generationCount;
//        int popSize = 10;
//        Individual specimen1;
//        Individual specimen2;
//        double mutationProbability = 0.1;
//        double maxMutationRate = 0.1;
        
//        List<Individual> offSprings = new List<Individual>();


//        // Use this for initialization
//        void Start()
//        {
//            this.ga = new GeneticAlgorythm();
//            System.Random random = new System.Random();
//           // ga.population.initPop();
//            ga.population.calculateFitness();
//            Console.WriteLine("Generation: " + ga.generationCount + "Fittest: " + ga.population.getFittestIndex());

//        }

//        // Update is called once per frame
//        void Update()
//        {
//            ga.generationCount++;
//            ga.selection();
//            ga.populateOffSpingsList();
//            ga.mutation();











//        }

//        void selection()
//        {
//            System.Random random = new System.Random();
//            int index1 = random.Next(0, population.getRouletteList().Count);
//            int index2 = random.Next(0, population.getRouletteList().Count);

//            Individual specimen1 = population.getRouletteList()[index1];
//            Individual specimen2 = population.getRouletteList()[index2];
//            Individual fittest = population.getFittest();


//        }
//        void crossover()
//        {
//            System.Random rn = new System.Random();

//            //Select a random crossover point
//            int crossOverPoint = rn.Next(population.getIndividuals()[0].getChromosomeLength());

//            //Swap values
//            for (int i = 0; i < crossOverPoint; i++)
//            {

//                specimen1.getChromosome()[i] = specimen2.getChromosome()[i];


//            }
//            offSprings.Add(specimen1);


//        }


//        void mutation()
//        {
//            System.Random rn = new System.Random();

//            foreach (Individual offspring in offSprings)
//            {
//                double chance = rn.NextDouble();

//                if (chance <= mutationProbability)
//                {
//                    //Select a random mutation point for specimen 1
//                    int mutationPoint = rn.Next(population.getIndividuals()[0].getChromosomeLength());
//                    //Define mutation rate
//                    double mutationRate = (rn.NextDouble() * (2 * maxMutationRate)) - maxMutationRate;
//                    //Mutate gene
//                    specimen1.getChromosome()[mutationPoint] += mutationRate;

//                }
//            }
//        }

   

//        void populateOffSpingsList()
//        {
//            offSprings.Add(fittest);
//            for (int i = 0; i < popSize; i++) ;
//            crossover();
//        }
        
//    }
//}