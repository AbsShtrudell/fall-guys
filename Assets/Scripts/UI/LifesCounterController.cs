using FallGuys;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifesCounterController : MonoBehaviour
{
    [Zenject.Inject]
    private Lifes _lifes;

    [SerializeField] private TMPro.TMP_Text _textMeshPro;

    private void OnEnable()
    {
        _lifes.LifesChanged += UpdateValue;
        UpdateValue(_lifes.CurrentLifes);
    }

    private void UpdateValue(int obj)
    {
        _textMeshPro.text = obj.ToString();
    }
}
