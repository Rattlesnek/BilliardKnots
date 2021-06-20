using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public enum ZoomSpeedType
{
    Constant,
    FasterWhenFurtherFromOrigin
}

/// <summary>
/// Camera orbit manipulator.
/// </summary>
/// <remarks>
/// Control keys:
/// Right mouse button - Rotation around center of rotation
/// Scroll wheel - Zoom
/// Scroll wheel down - Set center of rotation
/// </remarks>
public class OrbitManipulator : MonoBehaviour
{
    /// <summary>
    /// Zooming speed.
    /// </summary>
    [Range(1, 100)]
    public float ZoomSpeed = 30;

    public ZoomSpeedType ZoomSpeedType;

    [Range(1, 100)]
    public float MoveSpeed = 1;

    public Transform Gizmo;

    private bool drawGizmo = false;

    private bool lastDrawGizmo = false;

    private int drawGizmoNumFrames = 0;

    private Vector3 previousViewportPosition = Vector3.zero;

    private Vector3 previousWorldPosition = Vector3.zero;

    private Vector3 rotationCenter = Vector3.zero;

    private float maxZoomDistance = 5000f;

    private bool isControlDisabled = false;

    /// <summary>
    /// Sets center of rotation to origin
    /// </summary>
    public void SetRotationCenterToOrigin()
    {
        ChangeRotationCenter(Vector3.zero);
    }

    private void Start()
    {
        foreach (var camera in Camera.allCameras)
        {
            camera.nearClipPlane = 0.001f;
            camera.farClipPlane = 2000;
        }

        if (Gizmo != null)
        {
            Gizmo.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Mouse position update
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            previousViewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            previousWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward);
        }

        // Disable controls during translation of camera to new rotation center
        if (isControlDisabled)
        {
            return;
        }

        drawGizmo = false;
        if (drawGizmoNumFrames > 0)
        {
            drawGizmo = true;
            drawGizmoNumFrames--; 
        }

        // Set center of rotation
        if (Input.GetMouseButton(2))
        {
            var direction = previousWorldPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward);

            // Focus camera on rotation center
            rotationCenter = rotationCenter + direction * MoveSpeed;
            RotateAroundCenter(Vector3.zero);
            previousWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward);
            drawGizmo = true;
        }
        else
        {
            // Rotation around center of rotation
            if (Input.GetMouseButton(1))
            {
                var direction = previousViewportPosition - Camera.main.ScreenToViewportPoint(Input.mousePosition);

                // Rotate camera in certain direction
                RotateAroundCenter(direction);
                previousViewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                drawGizmo = true;
            }

            // Zoom to center of rotation
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            {
                var distanceFromRotationCenter = Vector3.Distance(transform.position, rotationCenter);
                float zoomScale = 10.0f;
                if (ZoomSpeedType == ZoomSpeedType.FasterWhenFurtherFromOrigin)
                {
                    zoomScale = distanceFromRotationCenter / 2.0f;
                    if (zoomScale < 1.0f)
                    {
                        zoomScale = 1.0f;
                    }
                }
                var zoomDistance = Input.mouseScrollDelta.y * Time.unscaledDeltaTime * zoomScale * ZoomSpeed;
                // Zoom destination greater than 0 || Zoom destination smaller than maxZoomDistance
                if (distanceFromRotationCenter - zoomDistance > 1 && distanceFromRotationCenter - zoomDistance < maxZoomDistance)
                {
                    transform.Translate(new Vector3(0, 0, zoomDistance));
                }
                

                drawGizmoNumFrames = 60;
                drawGizmo = true;
            }
        }

        // Draw gizmo
        if (Gizmo != null)
        {
            if (drawGizmo != lastDrawGizmo)
            {
                Gizmo.gameObject.SetActive(drawGizmo);
                lastDrawGizmo = drawGizmo;
            }
            if (drawGizmo)
            {
                Gizmo.position = rotationCenter;
            }
        }
    }

    private void RotateAroundCenter(Vector3 direction)
    {
        var distanceFromCenter = Vector3.Distance(rotationCenter, transform.position);
        transform.position = rotationCenter;
        transform.Rotate(Vector3.right, direction.y * 180);
        transform.Rotate(Vector3.up, -direction.x * 180, Space.World);
        transform.Translate(Vector3.back * distanceFromCenter);
    }

    private void ChangeRotationCenter(Vector3 newRotationCenter)
    {
        var deltaCenterPos = newRotationCenter - rotationCenter;
        var newPosition = transform.position + deltaCenterPos;
        rotationCenter = newRotationCenter;

        //StartCoroutine(SmoothTranslation(newPosition));
    }

    private Vector3 CalculateStepVector(Vector3 currentPosition, Vector3 deltaPosition)
    {
        var stepVec = deltaPosition * 0.015f;
        var time = currentPosition.magnitude / deltaPosition.magnitude;
        return stepVec * (-5 * time * (time - 1) + 1f / 6);
    }

    private IEnumerator SmoothTranslation(Vector3 newPosition)
    {
        // Disable controls during translation
        isControlDisabled = true;
        var deltaPosition = newPosition - transform.position;
        var currentPosition = Vector3.zero;

        while (currentPosition.magnitude < deltaPosition.magnitude)
        {
            var stepVec = CalculateStepVector(currentPosition, deltaPosition);
            transform.position += stepVec;
            currentPosition += stepVec;
            yield return null;
        }
        transform.position = newPosition;
        isControlDisabled = false;
    }
}
