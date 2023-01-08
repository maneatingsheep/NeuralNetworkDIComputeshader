
using UnityEngine;

public class WeightMatrix {

    //private float[] _flatMatrix;
    private float[,] _matrix;
    public float[] Biases;

    public int FromSize;
    public int ToSize;
    
    public WeightMatrix(int fromSize, int toSize) {
        FromSize = fromSize;
        ToSize = toSize;
        //_flatMatrix = new float[sizeX * sizeY];
        _matrix = new float[fromSize, toSize];
        for (int i = 0; i < fromSize; i++) {
            for (int j = 0; j < toSize; j++) {
                //_flatMatrix[i * toSize + j] = Random.value - 0.5f;
                _matrix[i, j] = Random.value - 0.5f;
            }
        }

        Biases = new float[toSize];
        for (int i = 0; i < toSize; i++) {
            Biases[i] = Random.value - 0.5f;
            //Biases[i] = 0;
        }
    }

    public float GetValue(int from, int to) {
        //return _flatMatrix[from * ToSize + to];
        return _matrix[from, to];
    }

    public void SetValue(float val, int from, int to) {
        //_flatMatrix[from * ToSize + to] = val;
        _matrix[from, to] = val;
    }

    /*public float[] GetFlatMatrix() {
        return _flatMatrix;
    }*/
}
