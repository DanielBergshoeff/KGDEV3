using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WispNeuralNetworkGathering : WispNeuralNetwork {
   
    public Transform WispTargetPosition;

    private void Start() {
        layers = new int[] { 12, 8, 5, 2 }; //12 inputs and 2 outputs
        if (Instance == null)
            Instance = this;

        if (loadFromFile)
            LoadNeuralNetworks();
        else
            ResetNeuralNetworks();
    }

    // Update is called once per frame
    void Update() {
        if (isTraining == false) {
            //Fresh start
            if (generationNumber == 0 && neuralNetworks == null) {
                Debug.Log("Fresh start");
                InitEntityNeuralNetworks();
            }
            else if (generationNumber == 0)
                Debug.Log("Start from neuralnetworks");
            //Continue
            else {
                averageFitness = CalculateAverageFitness();
                bestFitness = CalculateBestFitness();

                bestFitnessNr.text = bestFitness.ToString();
                avgFitnessNr.text = averageFitness.ToString();

                neuralNetworks.Sort();
                for (int i = 0; i < populationSize / 2; i++) {
                    neuralNetworks[i] = new NeuralNetwork(neuralNetworks[i + (populationSize / 2)]);
                    neuralNetworks[i].Mutate();

                    neuralNetworks[i + (populationSize / 2)] = new NeuralNetwork(neuralNetworks[i + (populationSize / 2)]);
                }

                for (int i = 0; i < populationSize; i++) {
                    neuralNetworks[i].SetFitness(0f);
                }
            }

            DrawNeuralNetwork.NeuralNetworkDrawing(neuralNetworks[0]);

            generationNumber++;
            generationNr.text = generationNumber.ToString();
            isTraining = true;
            wispSpawnPosition.position = GridManager.Instance.GetRandomNode().vPosition + Vector3.up * 3;
            WispTargetPosition.position = GridManager.Instance.GetRandomNode().vPosition + Vector3.up * 3;
            CreateEntities();
            timer = 0.0f;
        }
        else {
            timer += Time.deltaTime;
            int entityCount = entities.Count;
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] == null)
                    entityCount--;
            }
            if (entityCount == 0 || timer >= maxTime)
                Timer();
        }
    }

    void Timer() {
        if (entities != null) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] != null)
                    entities[i].SetFitnessAndDestroy(null);
            }
        }
        isTraining = false;
    }

    void InitEntityNeuralNetworks() {
        if (populationSize % 2 != 0) {
            populationSize++;
        }

        neuralNetworks = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++) {
            NeuralNetwork neuralNetwork = new NeuralNetwork(layers);
            neuralNetwork.Mutate();
            neuralNetworks.Add(neuralNetwork);
        }
    }

    void CreateEntities() {
        entities = new List<WispAI>();
        
        for (int i = 0; i < populationSize; i++) {
            WispAI wispAI = Instantiate(entityPrefab, wispSpawnPosition.position, entityPrefab.transform.rotation).GetComponent<WispAI>();
            wispAI.InitGathering(neuralNetworks[i], WispTargetPosition);
            entities.Add(wispAI);
        }
    }
}
