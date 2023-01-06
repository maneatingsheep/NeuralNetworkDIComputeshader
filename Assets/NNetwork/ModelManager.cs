using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ModelManager : MonoBehaviour {
    
    private Model[][] _trainModels;

    private SettingsConfig _settingsConfig;

    private int _trainIndex = 0;

    private float _mutationChance;

    [Inject]
    private void Construct(SettingsConfig settingsConfig) {
        _settingsConfig = settingsConfig;
    }

    public void Setup(int sizeOfGen, NeuralNetwork template) {

        _trainModels = new Model[2][];
        for (int i = 0; i < _trainModels.Length; i++) {
            _trainModels[i] = new Model[sizeOfGen];
            for (int j = 0; j < sizeOfGen; j++) {
                _trainModels[i][j] = EmptyModel(template);
                _trainModels[i][j].ResetScores();
            }
        }

        _mutationChance = _settingsConfig.AvgMutatePerSpeciment;

        int totalWeights = _trainModels[0][0].GetWeightCount();
        _mutationChance /= (float)totalWeights;
    }


    private Model EmptyModel(NeuralNetwork network) {
        return new Model(network, _settingsConfig);
    }


    internal void BreedNextGen() {

        //find parents and breed
        for (int i = 0; i < _trainModels[0].Length; i++) {
            
            int parent1Index = FindParentIndex();
            if (parent1Index == -1) {
                Debug.Log("no parent found");
                return;
            }

            int parent2Index;
            
            do {
                parent2Index = FindParentIndex();
            } while (parent1Index == parent2Index);
            
            BreedModels(parent1Index, parent2Index, i);
        }

        FlipTrainingBreeding();
        
    }

    

    private void FlipTrainingBreeding() {
        _trainIndex = 1 - _trainIndex;
    }

    

    private int FindParentIndex() {
        float rand = Random.value;
        float count = 0;
        for (int i = 0; i < TrainModels.Length; i++) {
            count += TrainModels[i].Fitness;
            if (count >= rand) {
                return i;
            }
        }

        return -1;
    }

    private void BreedModels(int parent1Index, int parent2Index, int targetIndex) {

        Model parent1 = TrainModels[parent1Index];
        Model parent2 = TrainModels[parent2Index];
        Model target = _trainModels[1 - _trainIndex][targetIndex];

        for (int w = 0; w < parent1.Weights.Length; w++) {

            int fromSize = parent1.Weights[w].FromSize;
            int toSize = parent1.Weights[w].ToSize;
            
            for (int i = 0; i < fromSize; i++) {
                for (int o = 0; o < toSize; o++) {
                    float p1Val = parent1.Weights[w].GetValue(i, o);
                    float p2Val = parent2.Weights[w].GetValue(i, o);

                    float nextVal = GenerateNextValue(p1Val, p2Val);
           
                    target.Weights[w].SetValue(nextVal, i, o);
                }
            }

            for (int o = 0; o < toSize; o++) {
                target.Weights[w].Biases[o] = GenerateNextValue(parent1.Weights[w].Biases[o], parent2.Weights[w].Biases[o]);
            }

        }
    }

    private float GenerateNextValue(float p1, float p2) {
        //select one random
        float selection = (Random.value > 0.5f) ? p1 : p2;
        //mutation
        if (Random.value < _mutationChance) {
            selection += (Random.value - 0.5f) * _settingsConfig.MutationStrength;
        }
        return selection;
    }

    internal void ResetScores() {
        for (int m = 0; m < TrainModels.Length; m++) {
            TrainModels[m].ResetScores();
        }
    }

    public Model[] TrainModels {
        get => _trainModels[_trainIndex];
    }

}
