using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    public event Action ReachedEnd;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
            ReachedEnd?.Invoke();
    }
}
