using System;

public class Model 
{
    public float Score;
    public float Fitness;

    public WeightMatrix[] Weights;

    public Model(NeuralNetwork network, SettingsConfig settingsConfig) {
        
        Weights = new WeightMatrix[settingsConfig.NumOfHiddenLayers + 1];

        for (int i = 0; i < Weights.Length; i++) {
            NetworkLayer fromLayer = (i == 0) ? network.InputLayer : network.HiddenLayers[i - 1];
            NetworkLayer toLayer = (i == settingsConfig.NumOfHiddenLayers) ? network.OutputLayer : network.HiddenLayers[i];

            Weights[i] = new WeightMatrix(fromLayer._neurons.Length, toLayer._neurons.Length);
        }
    }

    public void Reset() {
        Score = 0;
        Fitness = 0f;
    }

    internal int GetWeightCount() {
        int count = 0;
        for (int i = 0; i < Weights.Length; i++) {
            count += Weights[i].SizeX * Weights[i].SizeY;
        }

        return count;
    }
}
