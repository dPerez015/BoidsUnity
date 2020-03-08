using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings3D : ScriptableObject
{
    public float minSpeed = 2;
    public float maxSpeed = 5;
    public float sightRadius = 3;
    public float avoidRadius = 1.5f;
    public float obsAvoidRadius = 1.0f;

    public float maxSteerForce = 3;

    [Range(0, 2.5f)]
    public float alignWeight = 1;
    [Range(0, 2.5f)]
    public float cohesionWeight = 1;
    [Range(0, 4.5f)]
    public float avoidanceWeight = 1;
    [Range(0, 10.0f)]
    public float obstacleAvoidanceWeight = 1;
}
