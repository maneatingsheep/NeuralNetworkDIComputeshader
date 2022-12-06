using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour {
    
    private Model[] _trainModelsA;
    private Model[] _trainModelsB;

    public Model[] TrainModels;
    private Model[] _breedModels;

    private float[] _normlizedScores;


    public void Setup(int sizeOfGen, NeuralNetwork template) {

        _normlizedScores = new float[sizeOfGen];


        _trainModelsA = new Model[sizeOfGen];
        _trainModelsB = new Model[sizeOfGen];
        for (int i = 0; i < sizeOfGen; i++) {
            _trainModelsA[i] = EmptyModel(template);
            _trainModelsB[i] = EmptyModel(template);
            _trainModelsA[i].Score = 0;
            _trainModelsB[i].Score = 0;
        }

        TrainModels = _trainModelsA;
        _breedModels = _trainModelsB;
}

    private Model EmptyModel(NeuralNetwork network) {
        return new Model(network);
    }


    internal void BreedNextGen() {

        //normlize score
        float totalScore = 0;
        for (int i = 0; i < TrainModels.Length; i++) {
            _normlizedScores[i] = TrainModels[i].Score;
            totalScore += _normlizedScores[i];
        }

        if (totalScore == 0) {
            for (int i = 0; i < TrainModels.Length; i++) {
                _normlizedScores[i] = 1f;
            }
        } else {
            for (int i = 0; i < TrainModels.Length; i++) {
                _normlizedScores[i] /= totalScore;
            }
        }

        //find parents and breed
        for (int i = 0; i < _breedModels.Length; i++) {
            
            int parent1Index = FindParentIndex();
            int parent2Index = 0;
            
            do {
                parent2Index = FindParentIndex();
            } while (parent1Index == parent2Index);
            
            BreedModels(parent1Index, parent2Index, i);
        }

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
        for (int i = 0; i < _normlizedScores.Length; i++) {
            count += _normlizedScores[i];
            if (count >= rand) {
                return i;
            }
        }

        return -1;
    }

    private void BreedModels(int parent1Index, int parent2Index, int targetIndex) {

        Model parent1 = TrainModels[parent1Index];
        Model parent2 = TrainModels[parent2Index];
        Model target = TrainModels[targetIndex];

        for (int w = 0; w < parent1.Weights.Length; w++) {

            int xSize = parent1.Weights[w].Matrix.GetLength(0);
            int ySize = parent1.Weights[w].Matrix.GetLength(1);
            
            for (int i = 0; i < xSize; i++) {
                for (int j = 0; j < ySize; j++) {
                    float p1Val = parent1.Weights[w].Matrix[i, j];
                    float p2Val = parent2.Weights[w].Matrix[i, j];
                    //select one random
                    float selection = (Random.value > 0.5f) ? p1Val : p2Val;
                    //mutation
                    selection = (Random.value > 0.01f) ? selection : Mathf.Clamp((selection + (Random.value - 0.5f) * 0.001f), 0f, 1f);

                    target.Weights[w].Matrix[i, j] = selection;
                }
            }
        }
        
    }

    
}
