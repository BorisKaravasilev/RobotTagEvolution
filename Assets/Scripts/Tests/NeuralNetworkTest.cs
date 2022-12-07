using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class NeuralNetworkTest : MonoBehaviour
{
    private NeuralNetwork neuralNetwork;
    
    // Start is called before the first frame update
    void Start()
    {
        neuralNetwork = new NeuralNetwork(new []{2,3,2});
        neuralNetwork.InitializeWeightsAndBiasesRandomly();
        var weightsAndBiasesInLayer = neuralNetwork.GetWeightsAndBiasesInLayer();
        
        print("Input layer: " + weightsAndBiasesInLayer[0]);
        print("Hidden layer: " + weightsAndBiasesInLayer[1]);
        
        var testInputs = new[] { 1.0, 1.0 };
        var output = neuralNetwork.ComputeOutput(testInputs);
        
        print("Output: " + output);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
