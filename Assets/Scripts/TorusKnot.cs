using UnityEngine;

[ExecuteInEditMode]
public class TorusKnot : BaseKnot
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
        return KnotUtils.GetLissajousToricPosition(time, MajorRadius, MinorRadius, P, P, N, Phase);
    }

    protected override float GetPeriodTime()
    {
        if (P * N == 0)
        {
            return 2 * Mathf.PI;
        }
        return 2 * Mathf.PI / KnotUtils.GCD(P, N);
    }

    //protected override Vector3 GetPosition(float time)
    //{
    //    float minorRadScale = Mathf.Cos(Q * time);
    //
    //    // x and y are swapped for better looking default knot
    //    float x = (MinorRadius.x * minorRadScale + MajorRadius.x) * Mathf.Cos(P * time);
    //    float z = (MinorRadius.y * minorRadScale + MajorRadius.y) * Mathf.Sin(P * time);
    //    float y = - MinorRadius.z * Mathf.Sin(Q * time);
    //
    //    return new Vector3(x, y, z);
    //}
}


