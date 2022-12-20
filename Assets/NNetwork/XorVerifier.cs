using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XorVerifier : IVerifier {

    private XorNetOutputView _outputView;

    private const int FIELD_SIZE = 4;

    public int GetInputSize => 2;

    public int GetOtputSize => 1;

    public int Repetitions => FIELD_SIZE * FIELD_SIZE;


    private float[,] _sampleGrid = new float[FIELD_SIZE, FIELD_SIZE];

    private float[] ins;
    private int _iIndex1;
    private int _iIndex2;

    public XorVerifier(XorNetOutputView xorNetOutputView) {
        _outputView = xorNetOutputView;
    }

    public void Init() {
        _outputView.Init(FIELD_SIZE);
        ins = new float[GetInputSize];
    }

    public void SetFitness(Model[] models) {
        foreach (var model in models) {
            //model.Fitness = Mathf.Clamp(model.Score, 0, 1);
            model.Fitness = model.Score / Repetitions;
        }
    }

    public float[] SetNewVerefication(bool isTraining, int repetition, int attempt) {
        
        _iIndex1 = Mathf.FloorToInt((float)repetition / FIELD_SIZE);
        _iIndex2 = repetition % FIELD_SIZE;

        float iVal1 = _iIndex1 / (float)(FIELD_SIZE - 1);
        float iVal2 = _iIndex2 / (float)(FIELD_SIZE - 1);

        ins[0] = iVal1;
        ins[1] = iVal2;
        return ins;
    }

    public float Verify(float[] networkResult) {

        float wantedResult = CalculateRes();

        _sampleGrid[_iIndex1, _iIndex2] = networkResult[0];
        //_sampleGrid[_iIndex1, _iIndex2] = wantedResult;


        float score;
        if (wantedResult > 0.5f) {
            score = networkResult[0];
        } else {
            score = 1 - networkResult[0];
        }

        //score = Mathf.Clamp(score, 0, 1);


        return score;
    }

    private float CalculateRes() {
        int operationRes = Mathf.RoundToInt(ins[0]) | Mathf.RoundToInt(ins[1]);
        return (float)operationRes;
    }

    public void VisualizeResult() {
        _outputView.ShowOutput(_sampleGrid);
    }
}
