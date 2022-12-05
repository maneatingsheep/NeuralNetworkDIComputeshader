
using UnityEngine;

public class WeightMatrix {

    public float[,] Matrix;

    public WeightMatrix(int sizeX, int sizeY) {
        Matrix = new float[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                Matrix[i, j] = Random.value;
            }
        }
    }
}
