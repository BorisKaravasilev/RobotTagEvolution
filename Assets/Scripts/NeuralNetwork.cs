using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public enum ActivationFunction
{
    None,
    Sigmoid
}

public class NeuralNetwork
{
    private int[] _neuronsInLayer;
    private Matrix<double>[] _weightsAndBiasesInLayer;
    private ActivationFunction[] _activationFunctionInLayer;
    
    /// <summary>
    /// Initializes the neural network with random values.
    /// </summary>
    /// <param name="neuronsInLayer">Defines the structure of the network, i.e. how many neurons are in each layer.
    /// The first and the last element define neurons in the input and output layers respectively.</param>
    public NeuralNetwork(int[] neuronsInLayer)
    {
        _neuronsInLayer = neuronsInLayer;
        SetActivationFunctions(ActivationFunction.None, ActivationFunction.Sigmoid);
        InitializeWeightsAndBiasesRandomly();
    }

    public void SetActivationFunctions(ActivationFunction hiddenLayers, ActivationFunction outputLayer)
    {
        if (_neuronsInLayer == null) return;
        
        int numberOfLayers = _neuronsInLayer.Length;
        _activationFunctionInLayer = new ActivationFunction[numberOfLayers];

        // Set the activation function of the hidden layers
        for (int i = 0; i < numberOfLayers - 1; i++)
        {
            _activationFunctionInLayer[i] = hiddenLayers;
        }

        // Set the activation function of the output layer
        _activationFunctionInLayer[numberOfLayers - 1] = outputLayer;
    }

    public void InitializeWeightsAndBiasesRandomly(double mean = 0.0, double standardDeviation = 1.0)
    {
        int numberOfLayers = _neuronsInLayer.Length;
        _weightsAndBiasesInLayer = new Matrix<double>[numberOfLayers - 1];

        for (int i = 0; i < numberOfLayers - 1; i++) // -1 because the output layer is not connected to any other 
        {
            int neuronsInLayer = _neuronsInLayer[i]; // matrix width - 1
            int neuronsInNextLayer = _neuronsInLayer[i + 1]; // matrix height

            var M = Matrix<double>.Build;
            _weightsAndBiasesInLayer[i] = M.Random(neuronsInLayer + 1, neuronsInNextLayer, new Normal(mean, standardDeviation));
        }
    }

    public void UpdateWeightsAndBiases(double[] chromosome)
    {
        int numberOfLayers = _neuronsInLayer.Length;
        _weightsAndBiasesInLayer = new Matrix<double>[numberOfLayers - 1];

        int nextMatrixStartIndex = 0;

        for (int i = 0; i < numberOfLayers - 1; i++) // -1 because the output layer is not connected to any other 
        {
            int neuronsInLayer = _neuronsInLayer[i]; // matrix width - 1
            int neuronsInNextLayer = _neuronsInLayer[i + 1]; // matrix height

            int matrixElementCount = neuronsInLayer * neuronsInNextLayer;
            var segment = new ArraySegment<double>(chromosome, nextMatrixStartIndex, matrixElementCount);
            nextMatrixStartIndex += matrixElementCount;
            
            double[] currentFlattenedMatrix = segment.Array;
            double [,] weightsAndBiases = ArrayFlattener.Unflatten(currentFlattenedMatrix, neuronsInLayer + 1, neuronsInNextLayer);
            
            _weightsAndBiasesInLayer[i] = DenseMatrix.OfArray(weightsAndBiases);
        }
    }

    public void ComputeOutput(Vector<double> input)
    {
        // TODO: Multiply matrix by vector + apply activation function to propagate values to next layer
    }
}