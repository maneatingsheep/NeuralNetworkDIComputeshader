using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTester : MonoBehaviour
{

    [SerializeField] private ComputeShader _compShader;

    private ComputeBuffer _inBuffer;
    private ComputeBuffer _outBuffer;
    private ComputeBuffer _matrixBuffer;

    private int _bufferSize = 3;
    private int _dataSize = 3;

    void Start()
    {
        _inBuffer = new ComputeBuffer(_bufferSize, sizeof(float));
        _outBuffer = new ComputeBuffer(_bufferSize, sizeof(float));
        _matrixBuffer = new ComputeBuffer(_bufferSize * _bufferSize, sizeof(float));
        _compShader.SetBuffer(0, "_InBuffer", _inBuffer);
        _compShader.SetBuffer(0, "_OutBuffer", _outBuffer);
        _compShader.SetBuffer(0, "_MatrixBuffer", _matrixBuffer);
        DoStuff();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            DoStuff();
        }
    }

    private void DoStuff() {
        float[] ins = new float[_dataSize];

        for (int i = 0; i < _dataSize; i++) {
            ins[i] = i;
        }



        float[] matrix = new float[_dataSize * _dataSize];
        for (int i = 0; i < matrix.Length; i++) {
            matrix[i] = i;
        }


        _inBuffer.SetData(ins);
        _matrixBuffer.SetData(matrix);
        _compShader.Dispatch(0, _dataSize, 1, 1);

        float[] outs = new float[_dataSize];
        _outBuffer.GetData(outs);
        

        for (int i = 0; i < _dataSize; i++) {
            print(outs[i]);
        }

       
    }
}
