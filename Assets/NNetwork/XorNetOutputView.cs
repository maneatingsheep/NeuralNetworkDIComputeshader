using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XorNetOutputView : MonoBehaviour {

    [SerializeField] private Image _outputImage;
    private Texture2D _texture;
    private Color[] _colors;

    private int _resolution = 100;
    private int _sampleResolution;

    private int _size = 4;

    internal void Init(int size) {
        _size = size;
        _resolution = Mathf.CeilToInt(_resolution / size) * size;
        _sampleResolution = _resolution / size;
    }
    internal void ShowOutput(float[,] sampleGrid) {
        if (_outputImage.sprite == null) {
            _texture = new Texture2D(_resolution, _resolution);
            _outputImage.sprite = Sprite.Create(_texture, new Rect(0, 0, _resolution, _resolution), Vector2.zero);
            _colors = new Color[_resolution * _resolution];
        }



        for (int i = 0; i < _size; i++) {
            for (int j = 0; j < _size; j++) {
                for (int ir = 0; ir < _sampleResolution; ir++) {
                    for (int jr = 0; jr < _sampleResolution; jr++) {
                        float val = sampleGrid[i, j];
                        int _pixelX = i * _sampleResolution + ir;
                        int _pixelY = j * _sampleResolution + jr;

                        _colors[_pixelX * _resolution + _pixelY] = new Color(val, val, val, 1);

                    }
                }
            }
        }


        _texture.SetPixels(_colors);
        _texture.Apply();

    }

    
}
