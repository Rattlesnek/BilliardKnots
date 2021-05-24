using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldDragListener : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField]
    private InputField inputField;
    private float initialValue;

    private void Start()
    {
        if (inputField == null || (inputField.contentType != InputField.ContentType.DecimalNumber && inputField.contentType != InputField.ContentType.IntegerNumber))
        {
            Debug.LogError("Can't use InputFieldDragListener here!");
            Destroy(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialValue = float.Parse(inputField.text);
        inputField.Select();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - eventData.pressPosition;
        float finalValue = initialValue + delta.x + delta.y;
        if (inputField.contentType == InputField.ContentType.DecimalNumber)
            inputField.text = finalValue.ToString();
        else
            inputField.text = ((int)finalValue).ToString();
    }
}
