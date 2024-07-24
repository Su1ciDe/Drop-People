using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class NWS_StylizedWater_PosSetter : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Renderer _particleRenderer;
    private static readonly int ParticlePos = Shader.PropertyToID("_ParticlePos");


    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _particleRenderer.material.SetVector(ParticlePos, transform.position);
    }
}
