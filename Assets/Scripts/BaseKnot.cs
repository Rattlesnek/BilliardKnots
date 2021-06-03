using UnityEngine;
using SplineMesh;

public abstract class BaseKnot
{
    public Spline Knot;

    public int NumOfNodes;

    public float Curvature;

    protected float periodTime;

    public abstract Vector3 GetPosition(float time);

    public void AddNode(Vector3 currPos, Vector3 direction)
    {
        var node = new SplineNode(currPos, direction);
        Knot.AddNode(node);

        //var angle = Vector3.Angle(node.Direction - node.Position, Vector3.up);
        //if (angle < 45 || angle > 135)
        //{
        //    node.Up = Vector3.right;
        //}
    }

    public void RemoveNodes()
    {
        Knot.IsLoop = false;
        for (int i = Knot.nodes.Count - 1; i >= 2; i--)
        {
            Knot.RemoveNode(Knot.nodes[i]);
        }
    }

    public virtual void ConstructKnot()
    {
        RemoveNodes();

        float dtime = periodTime / NumOfNodes;

        // Knot construction
        // First two nodes are there as default

        // First node
        var previousPos = GetPosition(0.0f - dtime);
        var currentPos = GetPosition(0.0f);
        float timeNext = dtime;
        var nextPos = GetPosition(timeNext);

        var direction = GetSmoothDirection(previousPos, currentPos, nextPos, Curvature);

        Knot.nodes[0].Position = currentPos;
        Knot.nodes[0].Direction = direction;

        previousPos = currentPos;
        currentPos = nextPos;

        // Second node
        timeNext += dtime;
        nextPos = GetPosition(timeNext);
        direction = GetSmoothDirection(previousPos, currentPos, nextPos, Curvature);

        Knot.nodes[1].Position = currentPos;
        Knot.nodes[1].Direction = direction;

        do
        {
            previousPos = currentPos;
            currentPos = nextPos;

            timeNext += dtime;
            nextPos = GetPosition(timeNext);
            direction = GetSmoothDirection(previousPos, currentPos, nextPos, Curvature);

            AddNode(currentPos, direction);

        } while (timeNext < periodTime);

        // Last node at the position of the first node
        AddNode(Knot.nodes[0].Position, Knot.nodes[0].Direction);

        Knot.IsLoop = true;
    }

    public virtual void UpdateKnot()
    {       
        float dtime = periodTime / NumOfNodes;

        // Knot recalculation
        var prevPrevPos = GetPosition(-2 * dtime);
        var previousPos = GetPosition(-dtime);
        var currentPos = GetPosition(0.0f);
        var previousDir = GetSmoothDirection(prevPrevPos, previousPos, currentPos, Curvature);
        var previousNormal = GetFirstNormal(previousDir);

        var res = Reflection(previousNormal, (new Vector3(10, 10, 10)).normalized);

        Vector3 nextPos;

        float timeNext = dtime;
        for (int i = 0; i < Knot.nodes.Count; i++)
        {
            nextPos = GetPosition(timeNext);
            var currentDir = GetSmoothDirection(previousPos, currentPos, nextPos, Curvature);
            var currentNormal = GetNormalRMF(previousPos, currentPos, previousDir, currentDir, previousNormal);

            Knot.nodes[i].Position = currentPos;
            Knot.nodes[i].Direction = currentDir;
            Knot.nodes[i].Up = currentNormal;

            previousPos = currentPos;
            previousDir = currentDir;
            currentPos = nextPos;
            timeNext += dtime;
        }
    }

    protected static Vector3 GetFirstNormal(Vector3 tangent)
    {
        // dot(normal, tangent) == 0
        // length(normal) == 1
        // normal.y == 0

        float y = 0.0f;
        float z = Mathf.Sqrt(1.0f / (1.0f + Mathf.Pow(tangent.z / tangent.x, 2)));
        float x = -(tangent.z / tangent.x) * z;
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
        var v2 = (currentTangent - Reflection(previousTangent, v1)).normalized;
        var normal = Reflection(Reflection(previousNormal, v1), v2);
        return normal;
    }

    protected static Vector3 Reflection(Vector3 toReflect, Vector3 reflectAbout)
    {
        return Vector3.Reflect(-toReflect, reflectAbout);
    }

    protected static Vector3 GetSmoothDirection(
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
        dir = dir.normalized * averageMagnitude * curvature;

        // In SplineMesh, the node direction is not relative to the node position. 
        return dir + currentPos;
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
