using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVerifier {
    void Init();

    int GetInputSize { get; }
    int GetOtputSize { get; }

    float Verify(float[] networkResult, bool doVisualize);
    float[] SetNewVerefication(bool isTraining);

    void SetFitness(Model[] models);
    
}
