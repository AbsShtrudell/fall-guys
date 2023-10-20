using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _rechargeTime;

    private float _lastActive = 0;
    private bool _activated = false;

    [SerializeField] private Renderer _renderer1;
    [SerializeField] private Renderer _renderer2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
            ActivateTrap();
    }

    private void Update()
    {
        AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
        float currentTime = animState.normalizedTime % 1;
        _renderer1.material.SetFloat("_TimePercent", currentTime);
        _renderer2.material.SetFloat("_TimePercent", currentTime);
    }

    private void ActivateTrap()
    {
        if (_activated == true && Time.time < _lastActive + _rechargeTime) return;

        _animator.SetTrigger("Enable");
        _activated = true;
    }

    public void EnableTrap()
    {
        _collider.enabled = true;
    }

    public void DisableTrap()
    {
        _collider.enabled = false;

        _activated = false;
        _lastActive = Time.time;
    }
}
