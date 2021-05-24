using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeInspectorNamespace;

public class HideObjects : MonoBehaviour
{
    public List<Transform> HiddenObjects = new List<Transform>();

    public List<Component> HiddenComponents = new List<Component>();

    public List<string> HiddenVariables = new List<string>();
    
    public RuntimeHierarchy RuntimeHierarchy;

    public RuntimeInspector RuntimeInspector;

    public LissajousKnots lissKnots;

    // Start is called before the first frame update
    void Start()
    {
        RuntimeHierarchy.GameObjectFilter = GameObjectFilter;
        RuntimeInspector.ComponentFilter = ComponentFilter;
        RuntimeInspector.VariableFilter = VariableFilterLiss;
    }

    // Update is called once per frame
    void Update()
    {
        if (lissKnots.KnotType == KnotType.Lissajous)
        {
            RuntimeInspector.VariableFilter = VariableFilterLiss;
        }
        else
        {
            RuntimeInspector.VariableFilter = VariableFilterToric;
            RuntimeInspector.Refresh();
        }
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

    private bool VariableFilterLiss(string variableName)
    {
        return true;
    }

    private bool VariableFilterToric(string variableName)
    {
        if (variableName == "UnityEngine.Vector3 Amplitude")
        {
            return false;
        }
        return false;
    }
}
