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
    private NetworkOutputDisaply _networkOutputDisaply;

    private bool _doRun = true;

    [Inject]
    public void Construct(InputDisplayView inputDisplayView,
        NetworkOutputDisaply networkOutputDisaply,
        NeuralNetwork neuralNetwork,
        Trainer trainer,
        SettingsConfig settingsConfig,
        IVerifier verifier) {
        _neuralNetwork = neuralNetwork;
        _networkOutputDisaply = networkOutputDisaply;
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
        _doRun = true;
        for (int i = 0; i < _settingsConfig.GenerationsToRun; i++) {
            await _trainer.TrainOneGen();
            await Task.Yield();
            if (!_doRun) {
                break;
            }
        }
    }

    public void StopTrain() {
        _doRun = false;
    }

    public async void ShowImage() {

        var input = _verifier.SetNewVerefication(false);
        
        Model bestModel = _trainer.GetBestModel();
        _neuralNetwork.SetModel(bestModel);
        _neuralNetwork.SetInput(input);
        await _neuralNetwork.Run();

        _verifier.Verify(_neuralNetwork.OutputLayer._neurons, true);
    }

    private void OnDestroy() {
        _neuralNetwork.Dispose();
    }
}
