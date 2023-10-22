using DG.Tweening;
using FallGuys;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifesCounterController : MonoBehaviour
{
    [Zenject.Inject]
    private Lifes _lifes;

    [SerializeField] private RectTransform _iconRect;

    [SerializeField] private TMPro.TMP_Text _textMeshPro;

    private void OnEnable()
    {
        _lifes.LifesChanged += UpdateValue;
        UpdateValue(_lifes.CurrentLifes);
    }

    private void UpdateValue(int obj)
    {
        _iconRect.DOShakeScale(0.5f, 0.5f, 10);

        _textMeshPro.text = obj.ToString();
    }
}
