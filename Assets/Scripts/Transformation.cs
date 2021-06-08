using UnityEngine;

public class Transformation : MonoBehaviour
{
    [SerializeField]
    private Vector3 position;
    public Vector3 Position
    {
        get { return position; }
        set
        {
            position = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private Vector3 rotation;
    public Vector3 Rotation
    {
        get { return rotation; }
        set
        {
            rotation = value;
            updateNextFrame = true;
        }
    }

    [SerializeField]
    private Vector3 scale;
    public Vector3 Scale
    {
        get { return scale; }
        set
        {
            scale = value;
            updateNextFrame = true;
        }
    }

    private bool updateNextFrame = false;

    private Quaternion oldLocalRotationQuat;

    public void Start()
    {
        position = transform.localPosition;
        rotation = transform.localEulerAngles;
        scale = transform.localScale;
        oldLocalRotationQuat = transform.localRotation;
    }

    public void Update()
    {
        // Update of values by user
        if (updateNextFrame)
        {
            transform.localPosition = Position;
            transform.localEulerAngles = ConvertToEulerAngles(Rotation);
            transform.localScale = Scale;
            oldLocalRotationQuat = transform.localRotation;
            updateNextFrame = false;
        }
        // Update of transform values by editor or another obejct
        else if (transform.localPosition != Position ||
                transform.localRotation != oldLocalRotationQuat ||
                transform.localScale != Scale)
        {
            position = transform.localPosition;
            rotation = transform.localEulerAngles;
            scale = transform.localScale;
        }
    }

    private static Vector3 ConvertToEulerAngles(Vector3 angle)
    {
        return new Vector3(
             angle.x % 360,
             angle.y % 360,
             angle.z % 360);
    }
}
