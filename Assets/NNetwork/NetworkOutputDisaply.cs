using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkOutputDisaply : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void ShowOutput(int value) {
        _text.text = value.ToString();
    }
}
