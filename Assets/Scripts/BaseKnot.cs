using UnityEngine;
using SplineMesh;
using UnityEditor;

public abstract class BaseKnot : MonoBehaviour
{
    private Spline knot;
    protected Spline Knot
    {
        get
        {
            if (knot == null) knot = GetComponent<Spline>();
            return knot;
        }
    }

    [Range(40, 150)]
    public int NumOfNodes = 60;

    private int oldNumOfNodes;

    [Range(0, 1f)]
    public float Curvature = 0.3f;

    private float oldCurvature;

    [Range(0, 360f)]
    public float RMFAngle = 0f;

    private float oldRMFAngle;

    [SerializeField]
    private bool fixKnotEnds = true;
    public bool FixKnotEnds
    {
        get { return fixKnotEnds; }
        set
        {
            fixKnotEnds = value;
            updateNextFrame = true;
        }
    }

    protected float periodTime;

    protected bool constructNextFrame = false;

    protected bool updateNextFrame = false;


    protected abstract Vector3 GetPosition(float time);

    protected virtual void Start()
    {
        oldNumOfNodes = NumOfNodes;
        oldCurvature = Curvature;
        oldRMFAngle = RMFAngle;
    }
    
    protected virtual void Update()
    {      
        if (constructNextFrame || NumOfNodes != oldNumOfNodes)
        {
            ConstructKnot();
            oldNumOfNodes = NumOfNodes;
            constructNextFrame = false;
        }
        else if (updateNextFrame || Curvature != oldCurvature || RMFAngle != oldRMFAngle)
        {
            UpdateKnot();
            oldCurvature = Curvature;
            oldRMFAngle = RMFAngle;
            updateNextFrame = false;
        }
    }

    protected virtual void OnEnable()
    {
        ConstructKnot();
    }

    //protected virtual void OnValidate()
    //{
    //    if (!EditorApplication.isPlayingOrWillChangePlaymode)
    //    {
    //        updateNextFrame = true;
    //        Update();
    //    }
    //}

    protected virtual void ConstructKnot()
    {
        Knot.IsLoop = false;
        RemoveNodes();

        float dtime = periodTime / NumOfNodes;

        // Knot construction
        // First two nodes are there as default

        // First node
        var previousPos = GetPosition(-dtime);
        var currentPos = GetPosition(0.0f);
        float timeNext = dtime;
        var nextPos = GetPosition(timeNext);
        var (currentDir, currentTangent) = GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
        var currentNormal = GetFirstNormal(currentTangent);
        currentNormal = RotateAroundAxis(currentNormal, currentTangent, Mathf.Deg2Rad * RMFAngle);

        Knot.nodes[0].Position = currentPos;
        Knot.nodes[0].Direction = currentDir;
        Knot.nodes[0].Up = currentNormal;

        var previousNormal = currentNormal;
        var previousTangent = currentTangent;
        previousPos = currentPos;
        currentPos = nextPos;

        // Second node
        timeNext += dtime;
        nextPos = GetPosition(timeNext);
        (currentDir, currentTangent) = GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
        currentNormal = GetNormalRMF(previousPos, currentPos, previousTangent, currentTangent, previousNormal);

        Knot.nodes[1].Position = currentPos;
        Knot.nodes[1].Direction = currentDir;
        Knot.nodes[1].Up = currentNormal;

        do
        {
            previousNormal = currentNormal;
            previousTangent = currentTangent;
            previousPos = currentPos;
            currentPos = nextPos;

            timeNext += dtime;
            nextPos = GetPosition(timeNext);
            (currentDir, currentTangent) = GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
            currentNormal = GetNormalRMF(previousPos, currentPos, previousTangent, currentTangent, previousNormal);

            AddNode(currentPos, currentDir, currentNormal);

        } while (timeNext < periodTime - dtime / 2);

        // Last node at the position of the first node
        AddNode(Knot.nodes[0].Position, Knot.nodes[0].Direction, Knot.nodes[0].Up);
        Knot.IsLoop = true;
    }

    protected virtual void UpdateKnot()
    {
        float dtime = periodTime / (Knot.nodes.Count - 1);

        // Knot recalculation
        var previousPos = GetPosition(-dtime);
        var currentPos = GetPosition(0.0f);
        var nextPos = GetPosition(dtime);
        var (currentDir, currentTangent) = GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
        var currentNormal = GetFirstNormal(currentTangent);
        currentNormal = RotateAroundAxis(currentNormal, currentTangent, Mathf.Deg2Rad * RMFAngle);

        Knot.nodes[0].Position = currentPos;
        Knot.nodes[0].Direction = currentDir;
        Knot.nodes[0].Up = currentNormal;

        var previousNormal = currentNormal;
        var previousTangent = currentTangent;
        previousPos = currentPos;
        currentPos = nextPos;

        float timeNext = 2 * dtime;
        for (int i = 1; i < Knot.nodes.Count; i++, timeNext += dtime)
        {
            nextPos = GetPosition(timeNext);
            (currentDir, currentTangent) = GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
            currentNormal = GetNormalRMF(previousPos, currentPos, previousTangent, currentTangent, previousNormal);

            Knot.nodes[i].Position = currentPos;
            Knot.nodes[i].Direction = currentDir;
            Knot.nodes[i].Up = currentNormal;

            previousNormal = currentNormal;
            previousTangent = currentTangent;
            previousPos = currentPos;
            currentPos = nextPos;
        }

        if (FixKnotEnds)
        {
            ConnectKnotEnds(Knot.nodes[0].Up, currentNormal, currentTangent);
        }
    }

    protected void ConnectKnotEnds(Vector3 firstNormal, Vector3 lastNormal, Vector3 lastTangent)
    {
        // Ends dont match
        float totalAngle = Vector3.Angle(firstNormal, lastNormal) * Mathf.Deg2Rad;
        if (totalAngle > Knot.nodes.Count * Mathf.Epsilon)
        {
            var normalCross = Vector3.Cross(Knot.nodes[0].Up, lastNormal);
            var diff = (normalCross.normalized - lastTangent.normalized);
            const float epsilon = 0.0001f;
            if (diff.x > epsilon || diff.y > epsilon || diff.z > epsilon)
            {
                totalAngle = -totalAngle;
            }
            float angle = totalAngle / Knot.nodes.Count;

            // Rotate all
            for (int i = 0; i < Knot.nodes.Count; i++)
            {
                var node = Knot.nodes[i];
                node.Up = RotateAroundAxis(node.Up, (node.Direction - node.Position).normalized, totalAngle / 2 - i * angle);
            }
        }
    }

    protected void AddNode(Vector3 position, Vector3 direction, Vector3 up)
    {
        var node = new SplineNode(position, direction);
        node.Up = up;
        Knot.AddNode(node);
    }

    protected void RemoveNodes()
    {       
        for (int i = Knot.nodes.Count - 1; i >= 2; i--)
        {
            Knot.RemoveNode(Knot.nodes[i]);
        }
    }

    protected static Vector3 RotateAroundAxis(Vector3 vector, Vector3 normal, float angle)
    {
        var scaledNormal = normal * Mathf.Sin(angle / 2);
        var rotation = new Quaternion(scaledNormal.x, scaledNormal.y, scaledNormal.z, Mathf.Cos(angle / 2));
        return rotation * vector;
    }

    protected static Vector3 GetFirstNormal(Vector3 tangent)
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
    protected static Vector3 GetNormalRMF(
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

    protected static (Vector3, Vector3) GetSmoothDirectionAndTangent(
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

    protected delegate float MathFunc(float operand);

    protected static Vector3 ApplyFunc(Vector3 input, MathFunc mathFunc)
    {
        return new Vector3(
            mathFunc(input.x),
            mathFunc(input.y),
            mathFunc(input.z));
    }
}
