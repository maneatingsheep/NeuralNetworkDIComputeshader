using System;
using UnityEngine;

public static class MathFunctions {

    public enum ActivationType {Linear, Relu, Tanh, Sigmond}

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

    

    internal static float ActivationFunc(float x, ActivationType actType) {
        switch (actType) {
            case ActivationType.Linear:
                return x;
            case ActivationType.Tanh:
                return Tanh(x);
            case ActivationType.Sigmond:
                return Sigmoid(x);
            case ActivationType.Relu:
                return Relu(x);
            default:
                return x; //linear is default
        }
    }

    internal static float ActivationFuncDerivative(float x, ActivationType actType) {
        switch (actType) {
            case ActivationType.Linear:
                return 1;
            case ActivationType.Tanh:
                return TanhDeriv(x);
            case ActivationType.Sigmond:
                return SigmoidDeriv(x);
            case ActivationType.Relu:
                return (x <=0)?0: 1;
            default:
                return 1; //linear is default
        }
    }


    private static float Relu(float x) {
        return Mathf.Max(0, x);
    }

    private static float Tanh(float x) {
        return 2 * Sigmoid(2 * x) - 1f;

    }
    private static float TanhDeriv(float x) {
        return (1 + x) * (1 - x);

    }

    private static float Sigmoid(float x) {
        return 1f / (1f + MathF.Pow(MathF.E, -x));
    }

    private static float SigmoidDeriv(float x) {
        return x * (1f - x);
    }

}
