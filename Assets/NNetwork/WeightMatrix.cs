
using UnityEngine;

public class WeightMatrix {

    private float[] _flatMatrix;
    public float[] Biases;

    public int SizeX;
    public int SizeY;
    
    public WeightMatrix(int sizeX, int sizeY) {
        SizeX = sizeX;
        SizeY = sizeY;
        _flatMatrix = new float[sizeX * sizeY];
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                _flatMatrix[i * SizeY + j] = Random.value - 0.5f;
            }
        }

        Biases = new float[sizeY];
        for (int i = 0; i < sizeY; i++) {
            Biases[i] = Random.value - 0.5f;
        }
    }

    public float GetValue(int x, int y) {
        return _flatMatrix[x * SizeY + y];
    }

    public void SetValue(float val, int x, int y) {
        _flatMatrix[x * SizeY + y] = val;
    }

    public float[] GetFlatMatrix() {
        return _flatMatrix;
    }
}
