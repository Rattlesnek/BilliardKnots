using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeInspectorNamespace;

public class GUIHandler : MonoBehaviour
{
    public List<Transform> HiddenObjects = new List<Transform>();

    public List<Component> HiddenComponents = new List<Component>();

    [SerializeField]
    private List<string> hiddenVariables = new List<string>();
    public List<string> HiddenVariables
    {
        get { return hiddenVariables; }
        set
        {
            hiddenVariables = value;
            RuntimeInspector.Regenerate();
        }
    }
    
    public RuntimeHierarchy RuntimeHierarchy;

    public RuntimeInspector RuntimeInspector;

    public Transform KnotTransform;

    public void Awake()
    {
        var knotSelector = KnotTransform.GetComponent<KnotSelector>();
        knotSelector.OnKnotTypeChanged += OnKnotTypeChanged;

        RuntimeHierarchy.GameObjectFilter = GameObjectFilter;
        RuntimeInspector.ComponentFilter = ComponentFilter;
        RuntimeInspector.VariableFilter = VariableFilter;
    }

    private void OnKnotTypeChanged(KnotSelectorEventArgs args)
    {
        var lissajous = KnotTransform.GetComponent<LissajousKnot>();
        var torus = KnotTransform.GetComponent<TorusKnot>();
        
        switch (args.KnotType)
        {
            case KnotType.Lissajous:
                HiddenComponents.Remove(lissajous);
                HiddenComponents.Add(torus);
                break;
            case KnotType.Torus:
                HiddenComponents.Remove(torus);
                HiddenComponents.Add(lissajous);
                break;
            default:
                break;
        }

        RuntimeInspector.Refresh();
        RuntimeInspector.Regenerate();
    }

    private bool GameObjectFilter(Transform transform)
    {
        do
        {
            if (HiddenObjects.Contains(transform))
            {
                return false;
            }
            transform = transform.parent;
        } while (transform != null);

        return true;
    }

    private void ComponentFilter(GameObject gameObject, List<Component> components)
    {
        components.RemoveAll(component => HiddenComponents.Contains(component));
    }

    private bool VariableFilter(string variableName)
    {
        return ! HiddenVariables.Contains(variableName);
    }
}
