
using System.Collections.Generic;
using System;

public class NeuralNetwork : IComparable<NeuralNetwork>{

    private int[] layers; //layers
    private float[][] neurons; //neuron matrix
    private float[][][] weights; //weight matrix
    private float fitness; //fitness of the network

	public NeuralNetwork(int[] layers) {
        this.layers = new int[layers.Length];

        for (int i = 0; i < layers.Length; i++) {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeights();
        
    }

    /// <summary>
    /// Deep copy constructor
    /// </summary>
    /// <param name="copyNetwork"></param>
    public NeuralNetwork(NeuralNetwork copyNetwork) {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++) {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();

        CopyWeights(copyNetwork.weights);
    }

    private void CopyWeights(float[][][] copyWeights) {
        for (int i = 0; i < weights.Length; i++) {
            for (int j = 0; j < weights[i].Length; j++) {
                for (int k = 0; k < weights[i][j].Length; k++) {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    /// <summary>
    /// Initializes the neurons
    /// </summary>
    private void InitNeurons() {
        //Neuron initialization
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++) { //run through all layers
            neuronsList.Add(new float[layers[i]]); //add layer to neuron list
        }

        neurons = neuronsList.ToArray(); // convert list to array
    }

    /// <summary>
    /// Gives random values to the weights between neurons
    /// </summary>
    private void InitWeights() {
        //Weight initialization
        List<float[][]> weightsList = new List<float[][]>();

        for (int i = 1; i < layers.Length; i++) {
            List<float[]> layerWeightsList = new List<float[]>();

            int neuronsInPreviousLayer = layers[i - 1];

            for (int j = 0; j < neurons[i].Length; j++) {
                float[] neuronWeights = new float[neuronsInPreviousLayer]; //neurons weights

                //set the weights randomly between 0.5 and -0.5
                for (int k = 0; k < neuronsInPreviousLayer; k++) {
                    //give random weights to neuron weights
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray();
    }


    public float[] FeedForward(float[] inputs) {
        //Add inputs to the neuron matrix
        for (int i = 0; i < inputs.Length; i++) {
            neurons[0][i] = inputs[i];
        }

        //Iterate over all neurons and compute feedforward values
        for (int i = 1; i < layers.Length; i++) {
            for (int j = 0; j < neurons[i].Length; j++) {

                float value = 0f;
                for (int k = 0; k < neurons[i-1].Length; k++) {
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum of all weights connections of this neuron weight their values in previous layer
                }

                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length - 1];
    }

    /// <summary>
    /// Mutate neural network
    /// </summary>
    public void Mutate() {
        for (int i = 0; i < weights.Length; i++) {
            for (int j = 0; j < weights[i].Length; j++) {
                for (int k = 0; k < weights[i][j].Length; k++) {
                    float weight = weights[i][j][k];

                    //Mutate weight value
                    float randomNumber = UnityEngine.Random.Range(0f, 1000f);


                    if (randomNumber <= 2f) { //flip sign of weight
                        weight *= -1f;
                    }
                    else if(randomNumber <= 4f) { //randomly assign weight between 0.5 and -0.5
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if(randomNumber <= 6f) { //randomly increase by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if(randomNumber <= 8f) { //randomly decrease by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }


                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fit) {
        fitness += fit;
    }

    public void SetFitness(float fit) {
        fitness = fit;
    }

    public float GetFitness() {
        return fitness;
    }

    public int CompareTo(NeuralNetwork other) {
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }
}
