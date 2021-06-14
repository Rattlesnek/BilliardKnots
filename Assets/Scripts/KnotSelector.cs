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

    private LissajousKnot lissajous;

    private TorusKnot torus;

    private LissajousToricKnot lissajousToric;

    private void Awake()
    {
        lissajous = GetComponent<LissajousKnot>();
        torus = GetComponent<TorusKnot>();
        lissajousToric = GetComponent<LissajousToricKnot>();
    }

    private void Start()
    { 
        ExecuteOnKnotTypeChanged();
    }

    private void ExecuteOnKnotTypeChanged()
    {
        lissajous.enabled = false;
        torus.enabled = false;
        lissajousToric.enabled = false;

        switch (KnotType)
        {
            case KnotType.Lissajous:
                lissajous.enabled = true;
                break;
            case KnotType.Torus:
                torus.enabled = true;
                break;
            case KnotType.LissajousToric:
                lissajousToric.enabled = true;
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
