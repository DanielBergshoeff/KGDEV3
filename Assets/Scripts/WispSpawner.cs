using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispSpawner : MonoBehaviour {

    public static List<WispAI> AllWisps;
    public GridManager gridManager;
    public float spawnTime = 3.0f;
    public int maxAmtWisps = 20;

    public GameObject WispPrefab;

    public NeuralNetworkCollection neuralNetworkCollection;
    public List<NeuralNetwork> neuralNetworks;

    private int populationSize = 20;
    private int[] layers = new int[] { 3, 5, 5, 2 }; //3 inputs and 2 outputs

    private float timer = 0.0f;

	// Use this for initialization
	void Start () {
        InitNeuralNetworks();

        if(AllWisps == null) {
            AllWisps = new List<WispAI>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (AllWisps.Count < maxAmtWisps) {
            timer -= Time.deltaTime;
            if (timer <= 0.0f) {
                SpawnWisp();
                timer = spawnTime;
            }
        }
	}

    private void SpawnWisp() {
        foreach (Grid grid in gridManager.grids) {
            List<Node> availableNodes = grid.GetAvailableNodes();
            if (availableNodes.Count > 0) {
                int rndNumber = UnityEngine.Random.Range(0, availableNodes.Count);
                WispAI wisp = Instantiate(WispPrefab, availableNodes[rndNumber].vPosition + new Vector3(0.0f, 3.5f, 0.0f), Quaternion.identity).GetComponent<WispAI>();
                AllWisps.Add(wisp);

                if (neuralNetworks.Count > 0) {
                    int rndNumber2 = UnityEngine.Random.Range(0, neuralNetworks.Count);
                    wisp.neuralNetworkRoam = neuralNetworks[rndNumber2];
                }
            }
        }
    }

    private void InitNeuralNetworks() {
        neuralNetworks = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize / 2; i++) {
            NeuralNetwork neuralNetwork = new NeuralNetwork(layers);
            neuralNetwork.Mutate();
            neuralNetworks.Add(neuralNetwork);
        }

        for (int i = 0; i < populationSize / 2; i++) {
            for (int j = 0; j < neuralNetworkCollection.neuralNetworkHolders[i].layers.Length; j++) {
                for (int k = 0; k < neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons.Length; k++) {
                    for (int l = 0; l < neuralNetworkCollection.neuralNetworkHolders[i].layers[j].neurons[k].weights.Length; l++) {
                        neuralNetworks[i].Weights[j][k][l] = neuralNetworkCollection.neuralNetworkHolders[i + populationSize / 2].layers[j].neurons[k].weights[l];
                    }
                }
            }
        }
    }
}
