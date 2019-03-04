using System.Collections;
using System.Collections.Generic;
using System;

public class MyGeneticAlgorithm<T> {

    public List<MyDNA<T>> Population { get; private set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public T[] BestGenes { get; private set; }

    public int Elitism;
    public float MutationRate;
    private List<MyDNA<T>> newPopulation;
    private Random random;
    private float fitnessSum;
    
    public MyGeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<int, T> getRandomGene, Func<int, float> fitnessFunction, int Elitism = 0, float mutationRate = 0.01f) {
        Generation = 1;
        MutationRate = mutationRate;
        this.Elitism = Elitism;
        Population = new List<MyDNA<T>>(populationSize);
        newPopulation = new List<MyDNA<T>>(populationSize);
        this.random = random;

        BestGenes = new T[dnaSize];

        for (int i = 0; i < populationSize; i++) {
            Population.Add(new MyDNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
        }
    }

    public void NewGeneration() {
        if(Population.Count <= 0) {
            return;
        }

        CalculateFitness();
        Population.Sort(CompareDNA);

        newPopulation.Clear();

        for (int i = 0; i < Population.Count; i++) {
            if (i < Elitism) {
                newPopulation.Add(Population[i]);
            }
            else {
                MyDNA<T> parent1 = ChooseParent();
                MyDNA<T> parent2 = ChooseParent();

                MyDNA<T> child = parent1.Crossover(parent2);

                child.Mutate(MutationRate);

                newPopulation.Add(child);
            }
        }

        List<MyDNA<T>> tempList = Population;
        Population = newPopulation;
        newPopulation = tempList;

        Generation++;
    }

    public int CompareDNA(MyDNA<T> a, MyDNA<T> b) {
        if(a.Fitness > b.Fitness) {
            return -1;
        }
        else if (a.Fitness < b.Fitness){
            return 1;
        }
        else {
            return 0;
        }
    }


    public void CalculateFitness() {
        fitnessSum = 0;
        MyDNA<T> best = Population[0];

        for (int i = 0; i < Population.Count; i++) {
            fitnessSum += Population[i].CalculateFitness(i);

            if(Population[i].Fitness > best.Fitness) {
                best = Population[i];
            }
        }

        BestFitness = best.Fitness;
        best.Genes.CopyTo(BestGenes, 0);
    }

    private MyDNA<T> ChooseParent() {
        double randomNumber = random.NextDouble() * fitnessSum;

        for (int i = 0; i < Population.Count; i++) {
            if(randomNumber < Population[i].Fitness) {
                return Population[i];
            }

            randomNumber -= Population[i].Fitness;
        }

        return null;
    }
}
