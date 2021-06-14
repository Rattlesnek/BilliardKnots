using UnityEngine;

public static class KnotUtils
{
    public static Vector3 GetLissajousToricPosition(
        float time,
        Vector2 majorRad,
        Vector3 minorRad,
        int p, int q, int n,
        float phase)
    {
        float minorRadScale = Mathf.Sin(q * time);
        // x and y are swapped for better looking default knot
        float x = (minorRad.x * minorRadScale + majorRad.x) * Mathf.Cos(n * time);
        float z = (minorRad.y * minorRadScale + majorRad.y) * Mathf.Sin(n * time);
        float y = (minorRad.z) * Mathf.Cos(p * (time + phase));

        return new Vector3(x, y, z);
    }

    public static int LCM(int a, int b)
    {
        return a * b / GCD(a, b);
    }

    public static int GCD(int a, int b)
    {
        if (a % b == 0) return b;
        return GCD(b, a % b);
    }

    public static Vector3 RotateAroundAxis(Vector3 vector, Vector3 normal, float angle)
    {
        var scaledNormal = normal * Mathf.Sin(angle / 2);
        var rotation = new Quaternion(scaledNormal.x, scaledNormal.y, scaledNormal.z, Mathf.Cos(angle / 2));
        return rotation * vector;
    }

    public static Vector3 GetFirstNormal(Vector3 tangent)
    {
        // dot(normal, tangent) == 0; nx*tx + ny*ty + nz*tz == 0
        // length(normal) == 1; nx^2 + ny^2 + nz^2 == 1
        // length(tangent) == 1
        var absTang = ApplyFunc(tangent, Mathf.Abs);
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        if (absTang.x >= absTang.y && absTang.x >= absTang.z) // X
        {
            // tangent.x is max
            // normal.y == 0
            z = Mathf.Sqrt(1.0f / (1.0f + Mathf.Pow(tangent.z / tangent.x, 2)));
            x = -z * tangent.z / tangent.x;
        }
        else if (absTang.y >= absTang.x && absTang.y >= absTang.z) // Y
        {
            // tangent.y is max
            // normal.z == 0
            x = Mathf.Sqrt(1.0f / (1.0f + Mathf.Pow(tangent.x / tangent.y, 2)));
            y = -x * tangent.x / tangent.y;
        }
        else // Z
        {
            // tangent.z is max
            // normal.x == 0
            y = Mathf.Sqrt(1.0f / (1.0f + Mathf.Pow(tangent.y / tangent.z, 2)));
            z = -y * tangent.y / tangent.z;
        }

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Ref: https://seth.rocks/projects/doublereflection/
    /// </summary>
    public static Vector3 GetNormalRMF(
        Vector3 previousPos,
        Vector3 currentPos,
        Vector3 previousTangent,
        Vector3 currentTangent,
        Vector3 previousNormal)
    {
        var v1 = (currentPos - previousPos).normalized;
        var v2 = (currentTangent - Vector3.Reflect(previousTangent, v1)).normalized;
        var normal = Vector3.Reflect(Vector3.Reflect(previousNormal, v1), v2);
        return normal;
    }

    public static (Vector3, Vector3) GetSmoothDirectionAndTangent(
        Vector3 previousPos,
        Vector3 currentPos,
        Vector3 nextPos,
        float curvature)
    {
        var dir = Vector3.zero;
        float averageMagnitude = 0;

        // For the direction, we need to compute a smooth vector.
        // Orientation is obtained by substracting the vectors to the previous and next way points,
        // which give an acceptable tangent in most situations.
        // Then we apply a part of the average magnitude of these two vectors, according to the smoothness we want.

        // Previous
        var toPrevious = currentPos - previousPos;
        averageMagnitude += toPrevious.magnitude;
        dir += toPrevious.normalized;

        // Next
        var toNext = currentPos - nextPos;
        averageMagnitude += toNext.magnitude;
        dir -= toNext.normalized;

        averageMagnitude *= 0.5f;
        // This constant should vary between 0 and 0.5, and allows to add more or less smoothness.
        var dirNorm = dir.normalized;
        dir = dirNorm * averageMagnitude * curvature;

        return (dir + currentPos, dirNorm);
    }

    public delegate float MathFunc(float operand);

    public static Vector3 ApplyFunc(Vector3 input, MathFunc mathFunc)
    {
        return new Vector3(
            mathFunc(input.x),
            mathFunc(input.y),
            mathFunc(input.z));
    }
}
