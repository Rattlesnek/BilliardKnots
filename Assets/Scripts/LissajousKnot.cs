﻿using UnityEngine;
using SplineMesh;

public class LissajousKnot : BaseKnot
{
    [SerializeField]
    private Vector3 amplitude;
    public Vector3 Amplitude
    {
        get { return amplitude; }
        set
        {
            amplitude = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private Vector3Int frequency;
    public Vector3Int Frequency
    {
        get { return frequency; }
        set
        {
            frequency = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private Vector3 phase;
    public Vector3 Phase
    {
        get { return phase; }
        set
        {
            phase = value;
            updateNextFrame = true;
        }
    }

    private void Awake()
    {
        periodTime = 2 * Mathf.PI;
        enabled = false;
    }

    protected override Vector3 GetPosition(float time)
    {
        var freq = new Vector3(Frequency.x, Frequency.y, Frequency.z);
        var phi = Phase * Mathf.Deg2Rad;

        return Vector3.Scale(Amplitude, ApplyFunc(freq * time + phi, Mathf.Cos));
    }    
}

