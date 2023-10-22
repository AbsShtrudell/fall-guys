using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLine : MonoBehaviour
{
    public event Action Crossed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
            Crossed?.Invoke();
    }
}
