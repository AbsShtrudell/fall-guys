using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private float _rotationPerSecond = 1;

    protected void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * _rotationPerSecond);
    }
}