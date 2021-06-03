using UnityEngine;
using SplineMesh;

public class LissajousKnot : BaseKnot
{
    public Vector3 Amplitude;

    public Vector3Int Frequency;

    public Vector3 Phase;

    public LissajousKnot()
    {
        periodTime = 2 * Mathf.PI;
    }

    public override Vector3 GetPosition(float time)
    {
        var freq = new Vector3(Frequency.x, Frequency.y, Frequency.z);
        var phi = Phase * Mathf.Deg2Rad;

        return Vector3.Scale(Amplitude, ApplyFunc(freq * time + phi, Mathf.Cos));
    }    
}

