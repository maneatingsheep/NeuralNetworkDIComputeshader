public class Model 
{
    public float Score;

    public WeightMatrix[] Weights;

    public Model(NeuralNetwork network) {
        Weights = new WeightMatrix[network.NUM_OF_HIDDEN_LAYERS + 1];
        for (int i = 0; i < Weights.Length; i++) {
            NetworkLayer fromLayer = (i == 0) ? network.InputLayer : network.HiddenLayers[i - 1];
            NetworkLayer toLayer = (i == network.NUM_OF_HIDDEN_LAYERS) ? network.OutputLayer : network.HiddenLayers[i];

            Weights[i] = new WeightMatrix(fromLayer._neurons.Length, toLayer._neurons.Length);
        }
    }
}
