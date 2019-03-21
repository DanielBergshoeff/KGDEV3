using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using System;

public class WispNeuralNetwork : MonoBehaviour {
    public static WispNeuralNetwork Instance;
    
    public GameObject entityPrefab;

    public Text generationNr;
    public Text bestFitnessNr;
    public Text avgFitnessNr;

    public float timePerGeneration = 15.0f;
    
    public Transform wispSpawnPosition;

    protected bool isTraining = false;
    public int populationSize = 20;
    protected int generationNumber = 0;
    protected int[] layers = new int[] { 3, 5, 5, 2 }; //3 inputs and 2 outputs
    protected List<NeuralNetwork> neuralNetworks;
    protected List<WispAI> entities;

    protected float averageFitness = 0.0f;
    protected float bestFitness = 0.0f;

    protected float currentSimulationSpeed = 1.0f;

    public float maxTime = 20.0f;
    public bool loadFromFile = false;
    public NeuralNetworkCollection neuralNetworkCollection;
    protected float timer = 0.0f;
    public string fileName;

    private void Start() {
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
                InitEntityNeuralNetworks();
            }
            //Continue
            else if(!loadFromFile){ 
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

            loadFromFile = false;

            DrawNeuralNetwork.NeuralNetworkDrawing(neuralNetworks[0]);

            generationNumber++;
            generationNr.text = generationNumber.ToString();
            isTraining = true;
            CreateEntities(); timer = 0.0f;
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
                    entities[i].SelfDestroy();
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
            wispAI.Init(neuralNetworks[i]);
            entities.Add(wispAI);
        }
    }

    protected float CalculateAverageFitness() {
        float total = 0.0f;
        for (int i = 0; i < neuralNetworks.Count; i++) {
            total += neuralNetworks[i].GetFitness();
        }
        return total / neuralNetworks.Count;
    }

    protected float CalculateBestFitness() {
        float highest = float.NegativeInfinity;
        for (int i = 0; i < neuralNetworks.Count; i++) {
            float tempfitness = neuralNetworks[i].GetFitness();
            if (tempfitness > highest)
                highest = tempfitness;
        }
        return highest;
    }

    public void SetSimulationSpeed(float speed) {
        Debug.Log("Set speed to " + speed.ToString());
        Time.timeScale = speed;
        currentSimulationSpeed = speed;
    }

    public void SaveNeuralNetworks() {
        NeuralNetworkCollection neuralNetworkCollection = new NeuralNetworkCollection();
        neuralNetworkCollection.generationNumber = generationNumber;

        neuralNetworkCollection.neuralNetworkHolders = new NeuralNetworkHolder[neuralNetworks.Count];
        for (int i = 0; i < neuralNetworks.Count; i++) {
            neuralNetworkCollection.neuralNetworkHolders[i] = new NeuralNetworkHolder();
            neuralNetworkCollection.neuralNetworkHolders[i].layers = new Layer[neuralNetworks[i].Weights.Length];
            for (int j = 0; j < neuralNetworks[i].Weights.Length; j++) {
                neuralNetworkCollection.neuralNetworkHolders[i].layers[j] = new Layer();
                neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons = new Neuron[neuralNetworks[i].Weights[j].Length];
                for (int k = 0; k < neuralNetworks[i].Weights[j].Length; k++) {
                    neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons[k] = new Neuron();
                    neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons[k].weights = new float[neuralNetworks[i].Weights[j][k].Length];
                    for (int l = 0; l < neuralNetworks[i].Weights[j][k].Length; l++) {
                        neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons[k].weights[l] = neuralNetworks[i].Weights[j][k][l];
                    }
                }
            }
        }

        string path = "Assets/Wisp/NeuralNetworks";
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NNCollection.asset");
        AssetDatabase.CreateAsset(neuralNetworkCollection, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void LoadNeuralNetworks() {
        if(neuralNetworkCollection != null) {
            InitEntityNeuralNetworks();
            generationNumber = neuralNetworkCollection.generationNumber;

            for (int i = 0; i < neuralNetworkCollection.neuralNetworkHolders.Length; i++) {
                for (int j = 0; j < neuralNetworkCollection.neuralNetworkHolders[i].layers.Length; j++) {
                    for (int k = 0; k < neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons.Length; k++) {
                        for (int l = 0; l < neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons[k].weights.Length; l++) {
                            neuralNetworks[i].Weights[j][k][l] = neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons[k].weights[l];
                        }
                    }
                }
            }
        }
    }

    public void ResetNeuralNetworks() {
        neuralNetworks = null;
    }
}


