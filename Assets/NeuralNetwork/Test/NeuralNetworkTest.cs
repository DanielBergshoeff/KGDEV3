using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuralNetworkTest : MonoBehaviour {

    public GameObject entityPrefab;
    public GameObject player;
    public Camera playerCam;

    public Text generationNr;

    public float timePerGeneration = 15.0f;

    private bool isTraining = false;
    private int populationSize = 20;
    private int generationNumber = 0;
    private int[] layers = new int[] { 4, 10, 10, 2 }; //4 inputs and 2 outputs
    private List<NeuralNetwork> neuralNetworks;
    private List<FollowingEntity> entities;

    private float currentSimulationSpeed = 1.0f;
	
	// Update is called once per frame
	void Update () {
		if(isTraining == false) {
            if(generationNumber == 0) {
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

        }/*
        else {
            foreach (FollowingEntity followingEntity in entities) {
                Vector3 screenPoint = playerCam.WorldToViewportPoint(followingEntity.transform.position);

                if (!(screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)) {
                    followingEntity.AddFitness(Time.deltaTime);
                }
            }
        }*/
    }

    void Timer() {
        isTraining = false;
    }

    void InitEntityNeuralNetworks() {
        if(populationSize % 2 != 0) {
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
                GameObject.Destroy(entities[i].gameObject);
            }
        }

        entities = new List<FollowingEntity>();

        Vector3 pos = new Vector3(UnityEngine.Random.Range(-25f, 25f), 0, UnityEngine.Random.Range(-25f, 25f));

        for (int i = 0; i < populationSize; i++) {
            FollowingEntity followingEntity = Instantiate(entityPrefab, pos, entityPrefab.transform.rotation).GetComponent<FollowingEntity>();
            followingEntity.Init(neuralNetworks[i], player.transform);
            entities.Add(followingEntity);
        }
    }

    public void SetSimulationSpeed(float speed) {
        Time.timeScale = speed;
        currentSimulationSpeed = speed;
    }
}
