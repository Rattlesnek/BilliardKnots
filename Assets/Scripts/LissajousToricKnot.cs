using UnityEngine;

[ExecuteInEditMode]
public class LissajousToricKnot : BaseKnot
{
    [SerializeField]
    private int p;
    public int P
    {
        get { return p; }
        set
        {
            p = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private int q;
    public int Q
    {
        get { return q; }
        set
        {
            q = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private int n;
    public int N
    {
        get { return n; }
        set
        {
            n = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private float phase;
    public float Phase
    {
        get { return phase; }
        set
        {
            phase = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private Vector2 majorRadius;
    public Vector2 MajorRadius
    {
        get { return majorRadius; }
        set
        {
            majorRadius = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private Vector3 minorRadius;
    public Vector3 MinorRadius
    {
        get { return minorRadius; }
        set
        {
            minorRadius = value;
            updateNextFrame = true;
        }
    }

    protected override Vector3 GetPosition(float time)
    {
        return KnotUtils.GetLissajousToricPosition(time, MajorRadius, MinorRadius, P, Q, N, Phase);
    }

    protected override float GetPeriodTime()
    {
        if (P * Q * N == 0)
        {
            return 2 * Mathf.PI;
        }
        return 2 * Mathf.PI / KnotUtils.GCD(KnotUtils.GCD(P, Q), N);
    }
}
