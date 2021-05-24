using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public enum KnotType
{
    Lissajous,
    Toric,
    LissajousToric
};

public class LissajousKnots : MonoBehaviour
{
    private Spline knot;
    private Spline Knot
    {
        get 
        {
            if (knot == null) knot = GetComponent<Spline>();
            return knot;
        }
    }

    public KnotType KnotType = KnotType.Lissajous;

    [SerializeField]
    private Vector3 amplitude;
    public Vector3 Amplitude
    {
        get { return amplitude; }
        set 
        {
            amplitude = value;
            UpdateLissajousKnot();
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
            UpdateLissajousKnot();
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
            UpdateLissajousKnot();
        }
    }

    [Range(40, 150)]
    public int NumOfNodes = 60;

    private int oldNumOfNodes;

    [Range(0, 1f)]
    public float Curvature = 0.3f;

    private float oldCurvature;


    private const float periodTime = 2 * Mathf.PI;

    // Start is called before the first frame update
    void Start()
    {
        ConsructLissajousKnot();
    }

    // Update is called once per frame
    void Update()
    {
        if (NumOfNodes != oldNumOfNodes)
        {
            oldNumOfNodes = NumOfNodes;
            ConsructLissajousKnot();
        }
        else if (Curvature != oldCurvature)
        {
            oldCurvature = Curvature;
            UpdateLissajousKnot();
        }
    }

    // Remove for final application
    private void OnValidate()
    {
        if (Application.isEditor)
        {
            ConsructLissajousKnot();
        }
    }

    private void UpdateLissajousKnot()
    {
        float dtime = periodTime / NumOfNodes;

        // Knot recalculation
        var previousPos = GetLissajousPosition(0.0f - dtime);
        var currentPos = GetLissajousPosition(0.0f);
        Vector3 nextPos;

        float timeNext = dtime;
        for (int i = 0; i < Knot.nodes.Count; i++)
        {
            nextPos = GetLissajousPosition(timeNext);
            var direction = GetSmoothDirection(nextPos, currentPos, previousPos, Curvature);

            Knot.nodes[i].Position = currentPos;
            Knot.nodes[i].Direction = direction;

            previousPos = currentPos;
            currentPos = nextPos;
            timeNext += dtime; 
        }
    }

    private void ConsructLissajousKnot()
    {
        RemoveNodes();

        float dtime = periodTime / NumOfNodes;

        // Knot construction
        // First two nodes are there as default

        // First node
        var previousPos = GetLissajousPosition(0.0f - dtime);
        var currentPos = GetLissajousPosition(0.0f);
        float timeNext = dtime;
        var nextPos = GetLissajousPosition(timeNext);

        var direction = GetSmoothDirection(nextPos, currentPos, previousPos, Curvature);

        Knot.nodes[0].Position = currentPos;
        Knot.nodes[0].Direction = direction;

        previousPos = currentPos;
        currentPos = nextPos;

        // Second node
        timeNext += dtime;
        nextPos = GetLissajousPosition(timeNext);
        direction = GetSmoothDirection(nextPos, currentPos, previousPos, Curvature);

        Knot.nodes[1].Position = currentPos;
        Knot.nodes[1].Direction = direction;

        do
        {
            previousPos = currentPos;
            currentPos = nextPos;

            timeNext += dtime;
            nextPos = GetLissajousPosition(timeNext);
            direction = GetSmoothDirection(nextPos, currentPos, previousPos, Curvature);

            AddNode(currentPos, direction);

        } while (timeNext < periodTime);

        // Last node at the position of the first node
        AddNode(Knot.nodes[0].Position, Knot.nodes[0].Direction);

        Knot.IsLoop = true;
    }

    private void RemoveNodes()
    {
        Knot.IsLoop = false;
        for (int i = Knot.nodes.Count - 1; i >= 2; i--)
        {
            Knot.RemoveNode(Knot.nodes[i]);
        }
    }

    private void AddNode(Vector3 currPosition, Vector3 direction)
    {
        var node = new SplineNode(currPosition, direction);

        Knot.AddNode(node);

        var angle = Vector3.Angle(node.Direction - node.Position, Vector3.up);
        if (angle < 45 || angle > 135)
        {
            node.Up = Vector3.right;
        }
    }

    private Vector3 GetLissajousPosition(float time)
    {
        var freq = new Vector3(Frequency.x, Frequency.y, Frequency.z);
        var phi = Phase * Mathf.Deg2Rad;       

        return Vector3.Scale(Amplitude, ApplyFunc(freq * time + phi, Mathf.Cos));
    }

    private static Vector3 GetSmoothDirection(Vector3 nextPos, Vector3 currentPos, Vector3 previousPos, float curvature)
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

    private delegate float MathFunc(float operand);

    private static Vector3 ApplyFunc(Vector3 input, MathFunc mathFunc)
    {
        return new Vector3(
            mathFunc(input.x),
            mathFunc(input.y),
            mathFunc(input.z));
    }
}
