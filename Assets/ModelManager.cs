using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{

    public Model EmptyModel(NeuralNetwork network) {
        return new Model();
    }

    internal Model[] BreedNextGen(Model[] models) {
        throw new NotImplementedException();
    }
}
