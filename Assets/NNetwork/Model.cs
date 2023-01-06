using System;

public class Model 
{
    public float Score;
    public float Fitness;

    public WeightMatrix[] Weights;

    public Model(NeuralNetwork network, SettingsConfig settingsConfig) {
        
        Weights = new WeightMatrix[settingsConfig.NumOfHiddenLayers + 1];

        for (int i = 0; i < Weights.Length; i++) {

            var fromLayer = network.Layers[i];
            var toLayer = network.Layers[i + 1];

            Weights[i] = new WeightMatrix(fromLayer.Length, toLayer.Length);
        }
    }

    public void ResetScores() {
        Score = 0;
        Fitness = 0f;
    }

    internal int GetWeightCount() {
        int count = 0;
        for (int i = 0; i < Weights.Length; i++) {
            count += Weights[i].FromSize * Weights[i].ToSize;
        }

        return count;
    }
}
