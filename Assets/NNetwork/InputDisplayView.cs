using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplayView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;

    private int Imagesize = 28;

    internal void ShowImage(DataImage image) {
        _text.text = image.Label.ToString();

        Color[] colors = new Color[Imagesize * Imagesize];

        for (int i = 0; i < Imagesize; i++) {
            for (int j = 0; j < Imagesize; j++) {
                float c = image.Data[Imagesize - i - 1, j];
                colors[i * Imagesize + j] = new Color(c, c, c);
            }
        }

        (_image.mainTexture as Texture2D).SetPixels(colors);
        (_image.mainTexture as Texture2D).Apply();
    }
}
