using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVerifier {
    void Init();

    int GetInputSize { get; }
    int GetOtputSize { get; }
    int Repetitions { get; }

    float Verify(float[] networkResult);
    float[] SetNewVerefication(bool isTraining, int repetition, int attempt);

    void SetFitness(Model[] models);
    void VisualizeResult();
}
