using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class FlowMaster : MonoBehaviour
{
    private Trainer _trainer;
    private SettingsConfig _settingsConfig;
    private IVerifier _verifier;
    private NeuralNetwork _neuralNetwork;

    private bool _doRun = false;

    [Inject]
    public void Construct(NeuralNetwork neuralNetwork,
        Trainer trainer,
        SettingsConfig settingsConfig,
        IVerifier verifier) {
        _neuralNetwork = neuralNetwork;
        _trainer = trainer;
        _settingsConfig = settingsConfig;
        _verifier = verifier;
    }

    void Start() {

        _verifier.Init();
        _neuralNetwork.Init(_verifier.GetInputSize, _verifier.GetOtputSize);
        _trainer.Init();

    }


    public async void StartTrain() {
        if (_doRun) return;

        _doRun = true;
        for (int i = 0; i < _settingsConfig.GenerationsToRun; i++) {
            await _trainer.TrainOneGen();
            ShowImage();
            await Task.Yield();
            if (!_doRun) {
                break;
            }
        }
        _doRun = false;

    }

    public void StopTrain() {
        _doRun = false;
    }

    public async void ShowImage() {

        Model bestModel = _trainer.GetBestModel();
        _neuralNetwork.SetModel(bestModel);

        for (int i = 0; i < _verifier.Repetitions; i++) {
            var input = _verifier.SetNewVerefication(false, i, 0);
            _neuralNetwork.SetInput(input);
            await _neuralNetwork.Run();
            _verifier.Verify(_neuralNetwork.OutputLayer._neurons);
        }

        _verifier.VisualizeResult();
    }

    private void OnDestroy() {
        _neuralNetwork.Dispose();
    }
}
