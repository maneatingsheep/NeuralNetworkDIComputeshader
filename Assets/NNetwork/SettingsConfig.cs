using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsConfig", order = 1)]

public class SettingsConfig : ScriptableObject {
    
    public int NumOfHiddenLayers;
    public int HiddenLayerSize;
    public float MutationStrength;
    public float AvgMutatePerSpeciment;

    public int NumberOfAttemptsPerTrain;
    public int GenerationSize;

    public int GenerationsToRun;
}
