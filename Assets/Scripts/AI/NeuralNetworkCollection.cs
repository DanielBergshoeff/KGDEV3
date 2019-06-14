using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNetworkCollection : ScriptableObject {

    public int generationNumber;
    public NeuralNetworkHolder[] neuralNetworkHolders;
}

[Serializable]
public class Neuron {
    public float[] weights;
}

[Serializable]
public class Layer {
    public Neuron[] neurons;
}

[Serializable]
public class NeuralNetworkHolder {
    public Layer[] layers;
}
