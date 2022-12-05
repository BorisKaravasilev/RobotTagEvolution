using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnityEngine;

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
        InitializeWeightsAndBiasesToZero();
    }
    
    public NeuralNetwork(int[] neuronsInLayer, double[][,] weightsAndBiasesMatrixes)
    {
        _neuronsInLayer = neuronsInLayer;
        SetActivationFunctions(ActivationFunction.None, ActivationFunction.Sigmoid);
        SetWeightsAndBiasesFromMatrixes(weightsAndBiasesMatrixes);
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
            _weightsAndBiasesInLayer[i] = M.Random(neuronsInNextLayer, neuronsInLayer + 1, new Normal(mean, standardDeviation));
        }
    }
    
    public void InitializeWeightsAndBiasesToZero()
    {
        int numberOfLayers = _neuronsInLayer.Length;
        _weightsAndBiasesInLayer = new Matrix<double>[numberOfLayers - 1];

        for (int i = 0; i < numberOfLayers - 1; i++) // -1 because the output layer is not connected to any other 
        {
            int neuronsInLayer = _neuronsInLayer[i]; // matrix width - 1
            int neuronsInNextLayer = _neuronsInLayer[i + 1]; // matrix height

            var M = Matrix<double>.Build;
            _weightsAndBiasesInLayer[i] = M.Dense(neuronsInNextLayer, neuronsInLayer + 1);
        }
    }

    public void SetWeightsAndBiasesFromMatrixes(double[][,] weightsAndBiasesMatrixes)
    {
        int numberOfLayers = _neuronsInLayer.Length;
        _weightsAndBiasesInLayer = new Matrix<double>[numberOfLayers - 1];
        
        for (int i = 0; i < numberOfLayers - 1; i++) // -1 because the output layer is not connected to any other 
        {
            _weightsAndBiasesInLayer[i] = Matrix<double>.Build.DenseOfArray(weightsAndBiasesMatrixes[i]);
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

    public Vector<double> ComputeOutput(Vector<double> input)
    {
        Vector<double> propagatedInput = input;
        int i = 0;
        
        foreach (Matrix<double> weightsAndBiasesMatrix in _weightsAndBiasesInLayer)
        {
            i++;
            var inputsWithOneForBiases = new double[propagatedInput.Count + 1];
            propagatedInput.ToArray().CopyTo(inputsWithOneForBiases, 1);
            var vectorWithOneForBiases = Vector<double>.Build.DenseOfArray(inputsWithOneForBiases);
            propagatedInput = weightsAndBiasesMatrix.Multiply(vectorWithOneForBiases);

            var toApplyActivationFunction = propagatedInput.ToArray();

            for (int j = 0; j < toApplyActivationFunction.Length; j++)
            {
                toApplyActivationFunction[j] = Sigmoid(toApplyActivationFunction[j]);
            }

            propagatedInput = Vector<double>.Build.DenseOfArray(toApplyActivationFunction);
        }

        return propagatedInput;
    }

    public Matrix<double>[] GetWeightsAndBiasesInLayer()
    {
        return _weightsAndBiasesInLayer;
    }
    
    public static double Sigmoid(double value) {
        return 1.0f / (1.0 + (double) Math.Exp(-value));
    }
}