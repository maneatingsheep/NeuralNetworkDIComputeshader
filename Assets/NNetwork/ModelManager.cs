using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ModelManager : MonoBehaviour {
    
    private Model[] _trainModelsA;
    private Model[] _trainModelsB;

    public Model[] TrainModels;
    private Model[] _breedModels;

    private SettingsConfig _settingsConfig;

    [Inject]
    private void Construct(SettingsConfig settingsConfig) {
        _settingsConfig = settingsConfig;
    }

    public void Setup(int sizeOfGen, NeuralNetwork template) {

        _trainModelsA = new Model[sizeOfGen];
        _trainModelsB = new Model[sizeOfGen];
        for (int i = 0; i < sizeOfGen; i++) {
            _trainModelsA[i] = EmptyModel(template);
            _trainModelsB[i] = EmptyModel(template);
            _trainModelsA[i].Reset();
            _trainModelsB[i].Reset();
        }

        TrainModels = _trainModelsA;
        _breedModels = _trainModelsB;
}

    private Model EmptyModel(NeuralNetwork network) {
        return new Model(network, _settingsConfig);
    }


    internal void BreedNextGen() {

        //find parents and breed
        for (int i = 0; i < _breedModels.Length; i++) {
            
            int parent1Index = FindParentIndex();
            int parent2Index;
            
            do {
                parent2Index = FindParentIndex();
            } while (parent1Index == parent2Index);
            
            BreedModels(parent1Index, parent2Index, i);
        }

        FlipTrainingBreeding();
        
    }

    private void FlipTrainingBreeding() {
        if (TrainModels == _trainModelsB) {
            TrainModels = _trainModelsA;
            _breedModels = _trainModelsB;
        } else {
            TrainModels = _trainModelsB;
            _breedModels = _trainModelsA;
        }
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
        Model target = _breedModels[targetIndex];

        for (int w = 0; w < parent1.Weights.Length; w++) {

            int xSize = parent1.Weights[w].SizeX;
            int ySize = parent1.Weights[w].SizeY;

            for (int j = 0; j < ySize; j++) {
                for (int i = 0; i < xSize; i++) {
                    float p1Val = parent1.Weights[w].GetValue(i, j);
                    float p2Val = parent2.Weights[w].GetValue(i, j);

                    float nextVal = GenerateNextValue(p1Val, p2Val);
           
                    target.Weights[w].SetValue(nextVal, i, j);
                }
                target.Weights[w].Biases[j] = GenerateNextValue(parent1.Weights[w].Biases[j], parent2.Weights[w].Biases[j]);
            }
        }
    }

    private float GenerateNextValue(float p1, float p2) {
        //select one random
        float selection = (Random.value > 0.5f) ? p1 : p2;
        //mutation
        if (Random.value < _settingsConfig.MutationChance) {
            selection += (Random.value - 0.5f) * _settingsConfig.MutationRate;
        }
        return selection;
    }
}
