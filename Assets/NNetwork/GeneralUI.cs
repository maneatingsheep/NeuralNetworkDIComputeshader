using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GeneralUI : MonoBehaviour
{
    private FlowMaster _flowMaster;

    [Inject]
    public void Construct(FlowMaster flowMaster) {
        _flowMaster = flowMaster;
    }

    public void StartTrainingClicked() {
        _flowMaster.StartTrain();
    }

    public void StopTrainingClicked() {
        _flowMaster.StopTrain();
    }

    public void ShowBestOutput() {
        _flowMaster.ShowBestOutput();
    }

    public void ShowCorrectOutput() {
        _flowMaster.ShowCorrectOutput();

    }
}
