using System;
using UnityEngine;

public static class MathFunctions {
    internal static void NormlizeVector(float[] vector) {
        float max = float.MinValue;
        float min = float.MaxValue;
        //normlize
        for (int oIndex = 0; oIndex < vector.Length; oIndex++) {
            max = Mathf.Max(max, vector[oIndex]);
            min = Mathf.Min(min, vector[oIndex]);
        }

        float deltaSize = max - min;
        float offset = 1f - (max / deltaSize);

        for (int oIndex = 0; oIndex < vector.Length; oIndex++) {
            vector[oIndex] = (vector[oIndex] / deltaSize) + offset;
        }
        
    }

    internal static float ActivationFunc(float weightedSum) {
        return Tanh(weightedSum);
        //return Relu(weightedSum);
    }

    private static float Relu(float x) {
        return Mathf.Max(0, x);
    }

    private static float Tanh(float x) {
        return 2 * Sigmoid(2 * x) - 1f;

    }

    private static float Sigmoid(float x) {
        return 1f / (1f + MathF.Pow(MathF.E, -x));
    }
   

}
