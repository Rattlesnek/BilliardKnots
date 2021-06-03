using UnityEngine;
using SplineMesh;
using System.Collections.Generic;
using System.Linq;

public enum KnotType
{
    Lissajous,
    Toric,
    LissajousToric
};

public class BilliardKnots : MonoBehaviour
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

    [SerializeField]
    private KnotType knotType = KnotType.Lissajous;
    public KnotType KnotType
    {
        get { return knotType; }
        set
        {
            knotType = value;
            constructNextFrame = true;
        }
    }

    /// <summary>
    /// Lissajous Knot
    /// </summary>

    [SerializeField]
    private Vector3 amplitude;
    public Vector3 Amplitude
    {
        get { return amplitude; }
        set 
        {
            amplitude = value;
            lissajousKnot.Amplitude = amplitude;
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
            lissajousKnot.Frequency = frequency;
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
            lissajousKnot.Phase = phase;
            updateNextFrame = true;
        }
    }
    
    /// <summary>
    /// Torus Knot
    /// </summary>

    [SerializeField]
    private int p;
    public int P
    {
        get { return p; }
        set
        {
            p = value;
            torusKnot.P = p;
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
            torusKnot.Q = q;
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
            torusKnot.MajorRadius = majorRadius;
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
            torusKnot.MinorRadius = minorRadius;
            updateNextFrame = true;
        }
    }


    [Range(40, 150)]
    public int NumOfNodes = 60;

    private int oldNumOfNodes;

    [Range(0, 1f)]
    public float Curvature = 0.3f;

    private float oldCurvature;

  
    private bool updateNextFrame = false;

    private bool constructNextFrame = true;

    private LissajousKnot lissajousKnot = new LissajousKnot();

    private TorusKnot torusKnot = new TorusKnot();


    public HideObjects HideObjects;

    private readonly List<string> hideHideObjects = new List<string> { "HideObjects" };

    private readonly List<string> hideTorus = new List<string> { "P", "Q", "MajorRadius", "MinorRadius" };

    private readonly List<string> hideLissajous = new List<string> { "Amplitude", "Frequency", "Phase" };


    // Start is called before the first frame update
    void Start()
    {
        KnotType = KnotType.Lissajous;
        
        lissajousKnot.Knot = Knot;
        lissajousKnot.NumOfNodes = NumOfNodes;
        lissajousKnot.Curvature = Curvature;
        lissajousKnot.Amplitude = Amplitude;
        lissajousKnot.Frequency = Frequency;
        lissajousKnot.Phase = Phase;

        torusKnot.Knot = Knot;
        torusKnot.NumOfNodes = NumOfNodes;
        torusKnot.Curvature = Curvature;
        torusKnot.P = P;
        torusKnot.Q = Q;
        torusKnot.MajorRadius = MajorRadius;
        torusKnot.MinorRadius = MinorRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (constructNextFrame || NumOfNodes != oldNumOfNodes)
        {
            switch (KnotType)
            {
                case KnotType.Lissajous:
                    lissajousKnot.NumOfNodes = NumOfNodes;
                    lissajousKnot.ConstructKnot();
                    HideObjects.HiddenVariables = hideHideObjects.Concat(hideTorus).ToList();
                    break;
                case KnotType.Toric:
                    torusKnot.NumOfNodes = NumOfNodes;
                    torusKnot.ConstructKnot();
                    HideObjects.HiddenVariables = (hideHideObjects.Concat(hideLissajous)).ToList();
                    break;
                default:
                    break;
            };

            oldNumOfNodes = NumOfNodes;
            constructNextFrame = false;
        }
        else if (updateNextFrame || Curvature != oldCurvature)
        {          
            switch (KnotType)
            {
                case KnotType.Lissajous:
                    lissajousKnot.Curvature = Curvature;
                    lissajousKnot.UpdateKnot();
                    break;
                case KnotType.Toric:
                    torusKnot.Curvature = Curvature;
                    torusKnot.UpdateKnot();
                    break;
                default:
                    break;
            }

            oldCurvature = Curvature;
            updateNextFrame = false;
        }
    }

    // Remove for final application
    private void OnValidate()
    {
        if (Application.isEditor)
        {
            switch (KnotType)
            {
                case KnotType.Lissajous:
                    lissajousKnot.Knot = Knot;
                    lissajousKnot.NumOfNodes = NumOfNodes;
                    lissajousKnot.Curvature = Curvature;
                    lissajousKnot.Amplitude = Amplitude;
                    lissajousKnot.Frequency = Frequency;
                    lissajousKnot.Phase = Phase;
                    break;
                case KnotType.Toric:
                    torusKnot.Knot = Knot;
                    torusKnot.NumOfNodes = NumOfNodes;
                    torusKnot.Curvature = Curvature;
                    torusKnot.P = P;
                    torusKnot.Q = Q;
                    torusKnot.MajorRadius = MajorRadius;
                    torusKnot.MinorRadius = MinorRadius;
                    break;
                default:
                    break;
            }

            updateNextFrame = true;
            Update();
        }
    }
}
