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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            initialValue = float.Parse(inputField.text);
            inputField.Select();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 delta = eventData.position - eventData.pressPosition;
            if (inputField.contentType == InputField.ContentType.DecimalNumber)
            {
                float finalValue = initialValue + delta.x;
                inputField.text = finalValue.ToString();
            }
            else
            {
                float finalValue = initialValue + delta.x * 0.1f;
                inputField.text = ((int)finalValue).ToString();
            }
        }
    }
}
