using System.Threading.Tasks;
using UnityEngine;

public class XorVerifier : IVerifier {

    private XorNetOutputView _outputView;
    private NeuralNetwork _neuralNetwork;

    private const int FIELD_SIZE = 4;

    public int GetInputSize => 2;

    public int GetOtputSize => 1;


    public XorVerifier(XorNetOutputView xorNetOutputView, NeuralNetwork neuralNetwork) {
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

    public async Task<float> Verify() {


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

                float wantedResult = CalculateWantedRes(_neuralNetwork.InputLayer);
                score += -Mathf.Abs(wantedResult - (Mathf.Clamp01(_neuralNetwork.OutputLayer[0])));

            }
        }

        return score;
    }

    private float CalculateWantedRes(float[] inputs) {
        int operationRes = (Mathf.RoundToInt(inputs[0]) ^ Mathf.RoundToInt(inputs[1])) & 1;
        //float operationRes = ins[0] * ins[1];
        return (float)operationRes;
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

                float wantedResult = CalculateWantedRes(_neuralNetwork.InputLayer);
                sampleGrid[i, j] = wantedResult; //show wanted result
            }

        }
        _outputView.ShowOutput(sampleGrid);
    }
}
