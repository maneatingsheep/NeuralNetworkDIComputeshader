public class Model 
{
    public float Score;

    public NetworkWeightsSingle[] AllWeights;

    public Model(NeuralNetwork network) {
        AllWeights = new NetworkWeightsSingle[network.NUM_OF_HIDDEN_LAYERS + 1];
        for (int i = 0; i < AllWeights.Length; i++) {
            NetworkLayer fromLayer = (i == 0) ? network.InputLayer : network.HiddenLayers[i - 1];
            NetworkLayer toLayer = (i == network.NUM_OF_HIDDEN_LAYERS) ? network.HiddenLayers[i] : network.OutputLayer;

            AllWeights[i] = new NetworkWeightsSingle(fromLayer._neurons.Length, toLayer._neurons.Length);
        }
    }

    public Model(Model model) {
        AllWeights = new NetworkWeightsSingle[model.AllWeights.Length];
        for (int i = 0; i < AllWeights.Length; i++) {
            AllWeights[i] = new NetworkWeightsSingle(model.AllWeights[i].Weights.GetLength(0), model.AllWeights[i].Weights.GetLength(1));
        }
    }
}
