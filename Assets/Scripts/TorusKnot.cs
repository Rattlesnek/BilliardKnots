using UnityEngine;
using SplineMesh;

public class TorusKnot : BaseKnot
{
    public int P;

    public int Q;

    public Vector2 MajorRadius;

    public Vector3 MinorRadius;

    public TorusKnot()
    {
        periodTime = 2 * Mathf.PI;
    }

    public override Vector3 GetPosition(float time)
    {
        float minorRadScale = Mathf.Cos(Q * time);

        // x and y are swapped for better looking default knot
        float x = (MinorRadius.x * minorRadScale + MajorRadius.x) * Mathf.Cos(P * time);
        float z = (MinorRadius.y * minorRadScale + MajorRadius.y) * Mathf.Sin(P * time);
        float y = - MinorRadius.z * Mathf.Sin(Q * time);

        return new Vector3(x, y, z);
    }

    //public override Vector3 GetPosition(float time)
    //{
    //    float radiusScaled = MinorRadius * Mathf.Cos(Q * time) + MajorRadius;

    //    float x = radiusScaled * Mathf.Cos(P * time);
    //    float z = radiusScaled * Mathf.Sin(P * time);
    //    float y = -MinorRadius * Mathf.Sin(Q * time);

    //    return new Vector3(x, y, z);
    //}

    public override void ConstructKnot()
    {
        periodTime = GetPeriodTime();

        base.ConstructKnot();
    }

    public override void UpdateKnot()
    {
        periodTime = GetPeriodTime();

        base.UpdateKnot();
    }

    private float GetPeriodTime()
    {
        if (P * Q == 0)
        {
            return 2 * Mathf.PI;
        }
        return 2 * Mathf.PI / GCD(P, Q);
    }

    private static int LCM(int a, int b)
    {
        return a * b / GCD(a, b);
    }

    private static int GCD(int a, int b)
    {
        if (a % b == 0) return b;
        return GCD(b, a % b);
    }
}
