using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawNeuralNetwork : MonoBehaviour {

    List<NeuralNetwork> neuralNetworks;

    public static DrawNeuralNetwork Instance;

    Vector3 pos;

    private void Start() {
        if (Instance == null)
            Instance = this;

        neuralNetworks = new List<NeuralNetwork>();
    }

    private void Update() {

    }

    public static void NeuralNetworkDrawing(NeuralNetwork nn) {
        Instance.neuralNetworks = new List<NeuralNetwork>();
        Instance.neuralNetworks.Add(nn);
    }

    private void OnDrawGizmos() {
        pos = transform.position;


        if (neuralNetworks != null) {
            for (int i = 0; i < neuralNetworks.Count; i++) {
                for (int j = 0; j < neuralNetworks[i].Layers.Length; j++) {
                    for (int k = 0; k < neuralNetworks[i].Layers[j]; k++) {
                        Gizmos.color = Color.white;
                        Gizmos.DrawCube(pos + i * Vector3.right * 100.0f + j * Vector3.right * 5 + k * Vector3.down * 2, Vector3.one);
                        if(j - 1 >= 0) {
                            for (int l = 0; l < neuralNetworks[i].Layers[j - 1]; l++) {
                                /*
                                if (Mathf.Abs(neuralNetworks[i].Weights[j-1][k][l]) > 0.5f)
                                    Gizmos.color = Color.red;
                                else
                                    Gizmos.color = Color.white;
                                */

                                Vector3 lineColor = Vector3.Lerp(new Vector3(Color.white.r, Color.white.g, Color.white.b), new Vector3(Color.red.r, Color.red.g, Color.red.b), Mathf.Abs(neuralNetworks[i].Weights[j - 1][k][l]));
                                Gizmos.color = new Color(lineColor.x, lineColor.y, lineColor.z);

                                Gizmos.DrawLine(pos + i * Vector3.right * 100.0f + j * Vector3.right * 5 + k * Vector3.down * 2, 
                                pos + i * Vector3.right * 100.0f + (j-1) * Vector3.right * 5 + l * Vector3.down * 2);
                            }
                        }
                    }
                }
            }
        }
    }
}
