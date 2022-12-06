
using UnityEngine;

public class WeightMatrix {

    private float[] _flatMatrix;

    public int SizeX;
    public int SizeY;

    public WeightMatrix(int sizeX, int sizeY) {
        SizeX = sizeX;
        SizeY = sizeY;
        _flatMatrix = new float[sizeX * sizeY];
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                _flatMatrix[i * SizeY + j] = Random.value;
            }
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
