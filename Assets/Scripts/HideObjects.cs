using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeInspectorNamespace;

public class HideObjects : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        RuntimeHierarchy.GameObjectFilter = GameObjectFilter;
        RuntimeInspector.ComponentFilter = ComponentFilter;
        RuntimeInspector.VariableFilter = VariableFilter;
    }

    // Update is called once per frame
    void Update()
    {
        
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
