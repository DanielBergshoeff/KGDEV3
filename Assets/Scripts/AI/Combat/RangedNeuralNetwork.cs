using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangedNeuralNetwork : MonoBehaviour {
    public static RangedNeuralNetwork Instance;

    public static GameObject Player;
    public GameObject MyPlayer;
    public GameObject entityPrefab;

    public Text generationNr;
    public Text bestFitnessNr;
    public Text avgFitnessNr;

    public float timePerGeneration = 15.0f;

    public Vector3 roomSize;

    private bool isTraining = false;
    public int populationSize = 20;
    private int generationNumber = 0;
    private int[] layers = new int[] { 4, 10, 10, 4 }; //4 inputs and 4 outputs
    private List<NeuralNetwork> neuralNetworks;
    private List<EnemyAI> entities;

    private float averageFitness = 0.0f;
    private float bestFitness = 0.0f;

    private float currentSimulationSpeed = 1.0f;

    private void Start() {
        if (Player == null)
            Player = MyPlayer;
        if (Instance == null)
            Instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (isTraining == false) {
            if (generationNumber == 0) {
                InitEntityNeuralNetworks();
            }
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

            generationNumber++;
            generationNr.text = generationNumber.ToString();
            isTraining = true;
            CreateEntities();

        }

        bool done = true;
        for (int i = 0; i < entities.Count; i++) {
            if (!entities[i].GetComponent<EnemyAI>().bulletArrived) {
                done = false;
                break;
            }
        }
        if (done)
            isTraining = false;
    }

    void Timer() {
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
        if (entities != null) {
            for (int i = 0; i < entities.Count; i++) {
                EnemyAI enemyAI = entities[i].GetComponent<EnemyAI>();
                foreach(GameObject go in enemyAI.lights) {
                    if (go != null) {
                        go.GetComponent<EnemyLight>().SelfDestruct();
                        Destroy(go);
                    }
                }
                GameObject.Destroy(entities[i].gameObject);
            }
        }

        entities = new List<EnemyAI>();

        Vector3 pos = new Vector3(10, 0, 5);
        //Vector3 pos = new Vector3(UnityEngine.Random.Range(-25f, 25f), 0, UnityEngine.Random.Range(-25f, 25f));

        for (int i = 0; i < populationSize; i++) {
            EnemyAI enemyAI = Instantiate(entityPrefab, pos, entityPrefab.transform.rotation).GetComponent<EnemyAI>();
            enemyAI.InitNeuralNetwork(neuralNetworks[i], Player.transform);
            entities.Add(enemyAI);
        }
    }

    private float CalculateAverageFitness() {
        float total = 0.0f;
        for (int i = 0; i < neuralNetworks.Count; i++) {
            total += neuralNetworks[i].GetFitness();
        }
        return total / neuralNetworks.Count;
    }

    private float CalculateBestFitness() {
        float highest = float.NegativeInfinity;
        for (int i = 0; i < neuralNetworks.Count; i++) {
            float tempfitness = neuralNetworks[i].GetFitness();
            if (tempfitness > highest)
                highest = tempfitness;
        }
        return highest;
    }

    public void SetSimulationSpeed(float speed) {
        Time.timeScale = speed;
        currentSimulationSpeed = speed;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        if (roomSize != Vector3.zero) {
            Gizmos.DrawWireCube(transform.position, roomSize);
        }
    }
}
