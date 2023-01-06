using System.Threading.Tasks;
using UnityEngine;

public class BasicVerifier : IVerifier {

    private XorNetOutputView _outputView;
    private NeuralNetwork _neuralNetwork;

    private const int FIELD_SIZE = 4;

    public int GetInputSize => 2;

    public int GetOtputSize => 1;


    public BasicVerifier(XorNetOutputView xorNetOutputView, NeuralNetwork neuralNetwork) {
        _outputView = xorNetOutputView;
        _neuralNetwork = neuralNetwork;
    }

    public void Init() {
        _outputView.Init(FIELD_SIZE);
    }

    public void SetFitness(Model[] models) {
        foreach (var model in models) {
            model.Fitness = model.Score;
        }
    }

    public void SetNewVerefication(bool isTraining) {
        
    }

    public async Task<float> EvaluateModel() {


        /*_neuralNetwork.InputLayer[0] = Random.value;
        _neuralNetwork.InputLayer[1] = Random.value;

        await _neuralNetwork.Run();

        float wantedResult = CalculateWantedRes(_neuralNetwork.InputLayer);
        float score = -Mathf.Abs(wantedResult - (Mathf.Clamp01(_neuralNetwork.OutputLayer[0])));*/

        float score = 0f;

        for (int i = 0; i < FIELD_SIZE; i++) {
            for (int j = 0; j < FIELD_SIZE; j++) {
                _neuralNetwork.InputLayer[0] = (float)i / (FIELD_SIZE - 1);
                _neuralNetwork.InputLayer[1] = (float)j / (FIELD_SIZE - 1);

                await _neuralNetwork.Run();

                float wantedResult = CalculateWantedRes(_neuralNetwork.InputLayer)[0];
                score += -Mathf.Abs(wantedResult - (Mathf.Clamp01(_neuralNetwork.OutputLayer[0])));

            }
        }

        return score;
    }

    public float[] CalculateWantedRes(float[] inputs) {
        int operationRes = (Mathf.RoundToInt(inputs[0]) | (1 -  Mathf.RoundToInt(inputs[1]))) & 1;
        //float operationRes = inputs[0] * inputs[1];
        return new float[] { (float)operationRes };
    }


    public async Task VisualizeSample() {
        var sampleGrid = new float[FIELD_SIZE, FIELD_SIZE];

        for (int i = 0; i < FIELD_SIZE; i++) {
            for (int j = 0; j < FIELD_SIZE; j++) {
                _neuralNetwork.InputLayer[0] = (float)i / (FIELD_SIZE - 1);
                _neuralNetwork.InputLayer[1] = (float)j / (FIELD_SIZE - 1);

                await _neuralNetwork.Run();

                sampleGrid[i, j] = _neuralNetwork.OutputLayer[0]; //show wanted result
            }
        }

        _outputView.ShowOutput(sampleGrid);
    }

    public void VisualizeOutputReference() {
        var sampleGrid = new float[FIELD_SIZE, FIELD_SIZE];

        for (int i = 0; i < FIELD_SIZE; i++) {
            for (int j = 0; j < FIELD_SIZE; j++) {

                _neuralNetwork.InputLayer[0] = (float)i / (FIELD_SIZE - 1);
                _neuralNetwork.InputLayer[1] = (float)j / (FIELD_SIZE - 1);

                float wantedResult = CalculateWantedRes(_neuralNetwork.InputLayer)[0];
                sampleGrid[i, j] = wantedResult; //show wanted result
            }

        }
        _outputView.ShowOutput(sampleGrid);
    }

}
