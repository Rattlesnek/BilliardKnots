using System;
using UnityEngine;

public enum KnotType
{
    Lissajous,
    Torus,
    LissajousToric
};

public class KnotSelector : MonoBehaviour
{
    [SerializeField]
    private KnotType knotType = KnotType.Lissajous;
    public KnotType KnotType
    {
        get { return knotType; }
        set
        {
            if (knotType != value)
            {
                knotType = value;
                ExecuteOnKnotTypeChanged();
            }
        }
    }

    public delegate void KnotTypeChangedEvent(KnotSelectorEventArgs args);

    public event KnotTypeChangedEvent OnKnotTypeChanged;

    private LissajousKnot lissajousKnot;

    private TorusKnot torusKnot;

    private void Awake()
    {
        lissajousKnot = GetComponent<LissajousKnot>();
        torusKnot = GetComponent<TorusKnot>();
    }

    private void Start()
    { 
        ExecuteOnKnotTypeChanged();
    }

    private void ExecuteOnKnotTypeChanged()
    {
        lissajousKnot.enabled = false;
        torusKnot.enabled = false;

        switch (KnotType)
        {
            case KnotType.Lissajous:
                lissajousKnot.enabled = true;
                break;
            case KnotType.Torus:
                torusKnot.enabled = true;
                break;
            default:
                break;
        }

        // Trigger event
        if (OnKnotTypeChanged != null)
        {
            OnKnotTypeChanged(new KnotSelectorEventArgs(KnotType));
        }
    }
}

public class KnotSelectorEventArgs : EventArgs
{
    public KnotType KnotType { get; set; }
    public KnotSelectorEventArgs(KnotType knotType)
    {
        this.KnotType = knotType;
    }
}
