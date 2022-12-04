
using UnityEngine;

public class NetworkWeightsSingle {

    public float[,] Weights;

    public NetworkWeightsSingle(int sizeX, int sizeY) {
        Weights = new float[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                Weights[i, j] = Random.value;
            }
        }
    }
}
