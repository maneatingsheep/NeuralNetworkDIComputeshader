using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLayer
{
    public float[] _neurons;
    public NetworkLayer(int size) {
        _neurons = new float[size];
    }
}
