using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDNetInputView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;


    internal void ShowImage(DataImage image, int imageSize) {
        _text.text = image.Label.ToString();

        Color[] colors = new Color[imageSize * imageSize];

        for (int i = 0; i < imageSize; i++) {
            for (int j = 0; j < imageSize; j++) {
                float c = image.Data[imageSize - i - 1, j];
                colors[i * imageSize + j] = new Color(c, c, c);
            }
        }

        (_image.mainTexture as Texture2D).SetPixels(colors);
        (_image.mainTexture as Texture2D).Apply();
    }
}
