using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyNeuralNetwork : MonoBehaviour {

    public GameObject EnemyControllerAIPrefab;
    public float timePerGeneration = 15.0f;
    public Text generationNr;

    private bool isTraining = false;
    public int populationSize = 20;
    private int generationNumber = 0;
    private int[] layers = new int[] { 3, 5, 5, 1 }; //2 inputs and 1 outputs
    private List<NeuralNetwork> neuralNetworks;
    private List<EnemyControllerAI> enemyControllerAIs;

    // Use this for initialization
    void Start () {
        
	}

    private void Update() {
        if (isTraining == false) {
            if (generationNumber == 0) {
                InitEntityNeuralNetworks();
            }
            else {
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
            Invoke("Timer", timePerGeneration);
            CreateEntities();
        }
    }

    private void CreateEntities() {
        if (enemyControllerAIs != null) {
            for (int i = 0; i < enemyControllerAIs.Count; i++) {
                GameObject.Destroy(enemyControllerAIs[i].gameObject);
            }
        }

        enemyControllerAIs = new List<EnemyControllerAI>();

        for (int i = 0; i < populationSize; i++) {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z + i * 50);
            EnemyControllerAI enemyControllerAI = Instantiate(EnemyControllerAIPrefab, pos, EnemyControllerAIPrefab.transform.rotation).GetComponent<EnemyControllerAI>();
            enemyControllerAI.Init(neuralNetworks[i]);
            enemyControllerAIs.Add(enemyControllerAI);
        }
    }

    void Timer() {
        isTraining = false;
    }

    private void InitEntityNeuralNetworks() {
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
}
