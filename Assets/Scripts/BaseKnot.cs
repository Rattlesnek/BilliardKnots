using UnityEngine;
using SplineMesh;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
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

    protected abstract float GetPeriodTime();


    protected virtual void Awake()
    {
        periodTime = GetPeriodTime();
        enabled = false;
    }

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
        periodTime = GetPeriodTime();
        ConstructKnot();
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            constructNextFrame = true;
        }
    }
#endif

    protected virtual void ConstructKnot()
    {
        RemoveNodes();

        periodTime = GetPeriodTime();
        float dtime = periodTime / NumOfNodes;

        // Knot construction
        // First two nodes are there as default

        // First node
        var previousPos = GetPosition(-dtime);
        var currentPos = GetPosition(0.0f);
        float timeNext = dtime;
        var nextPos = GetPosition(timeNext);
        var (currentDir, currentTangent) = KnotUtils.GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
        var currentNormal = KnotUtils.GetFirstNormal(currentTangent);
        currentNormal = KnotUtils.RotateAroundAxis(currentNormal, currentTangent, Mathf.Deg2Rad * RMFAngle);

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
        (currentDir, currentTangent) = KnotUtils.GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
        currentNormal = KnotUtils.GetNormalRMF(previousPos, currentPos, previousTangent, currentTangent, previousNormal);

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
            (currentDir, currentTangent) = KnotUtils.GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
            currentNormal = KnotUtils.GetNormalRMF(previousPos, currentPos, previousTangent, currentTangent, previousNormal);

            AddNode(currentPos, currentDir, currentNormal);

        } while (timeNext < periodTime + dtime / 2);

        if (FixKnotEnds)
        {
            ConnectKnotEnds(Knot.nodes[0].Up, currentNormal, currentTangent);
        }
    }

    protected virtual void UpdateKnot()
    {
        periodTime = GetPeriodTime();
        float dtime = periodTime / (Knot.nodes.Count - 1);

        // Knot recalculation
        var previousPos = GetPosition(-dtime);
        var currentPos = GetPosition(0.0f);
        var nextPos = GetPosition(dtime);
        var (currentDir, currentTangent) = KnotUtils.GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
        var currentNormal = KnotUtils.GetFirstNormal(currentTangent);
        currentNormal = KnotUtils.RotateAroundAxis(currentNormal, currentTangent, Mathf.Deg2Rad * RMFAngle);

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
            (currentDir, currentTangent) = KnotUtils.GetSmoothDirectionAndTangent(previousPos, currentPos, nextPos, Curvature);
            currentNormal = KnotUtils.GetNormalRMF(previousPos, currentPos, previousTangent, currentTangent, previousNormal);

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
                node.Up = KnotUtils.RotateAroundAxis(node.Up, (node.Direction - node.Position).normalized, totalAngle / 2 - i * angle);
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
}
