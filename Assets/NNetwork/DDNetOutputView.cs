using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DDNetOutputView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private int _res;

    public void SetResult(int res) {
        _res = res;
    }

    public void ShowOutput() {
        _text.text = _res.ToString();
    }
}
