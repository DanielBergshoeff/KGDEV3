using System.Collections;
using System.Collections.Generic;
using System;

public class MyDNA<T> {

	public T[] Genes { get; private set; }
    public float Fitness { get; private set; }

    private Random random;
    private Func<int, T> getRandomGene;
    private Func<int, float> fitnessFunction;

    public MyDNA(int size, Random random, Func<int, T> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true) {
        Genes = new T[size];
        this.random = random;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        if (shouldInitGenes) {
            for (int i = 0; i < Genes.Length; i++) {
                Genes[i] = getRandomGene(i);
                UnityEngine.Debug.Log(Genes[i]);
            }
        }
    }

    public float CalculateFitness(int index) {
        UnityEngine.Debug.Log(fitnessFunction);
        Fitness = fitnessFunction(index);
        return Fitness;
    }

    public MyDNA<T> Crossover(MyDNA<T> otherParent) {
        MyDNA<T> child = new MyDNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

        for (int i = 0; i < Genes.Length; i++) {
            child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];
        }

        return child;
    }

    public void Mutate(float mutationRate) {
        for (int i = 0; i < Genes.Length; i++) {
            if(random.NextDouble() < mutationRate) {
                Genes[i] = getRandomGene(i);
            }
        }
    }
}
