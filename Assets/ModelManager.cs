using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    private float[] _scores;

    public Model EmptyModel(NeuralNetwork network) {
        return new Model(network);
    }

    internal Model[] BreedNextGen(ref Model[] models) {

        _scores = new float[models.Length];
        
        float total = 0;
        for (int i = 0; i < models.Length; i++) {
            _scores[i] = models[i].Score;
            total += _scores[i];
        }

        for (int i = 0; i < models.Length; i++) {
            _scores[i] /= total;
        }

        var res = new Model[models.Length];

        for (int i = 0; i < models.Length; i++) {
            Model parent1 = FindParent(ref models);
            Model parent2;
            do {
                parent2 = FindParent(ref models);
            } while (parent1 == parent2);
            res = BreedModels(parent1, parent2);
        }


        return res;
    }

    private Model FindParent(ref Model[] models) {
        float rand = Random.value;
        float count = 0;
        for (int i = 0; i < _scores.Length; i++) {
            count += _scores[i];
            if (count >= rand) {
                return models[i];
            }
        }

        return null;
    }

    private Model[] BreedModels(Model parent1, Model parent2) {
        throw new NotImplementedException();
    }

    
}
