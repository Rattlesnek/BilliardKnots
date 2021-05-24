using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public float Speed = 30;

    private Vector3 previousPosition = Vector3.zero;

    private Vector3 rotationCenter = Vector3.zero;

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
            camera.farClipPlane = 1000;
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        // Mouse position update
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        // Disable controls during translation of camera to new rotation center
        if (isControlDisabled)
        {
            return;
        }

        // Zoom to center of rotation
        var distanceFromRotationCenter = Vector3.Distance(transform.position, rotationCenter);
        var nonlinearZoomScale = distanceFromRotationCenter / 2.0f;
        if (nonlinearZoomScale < 1.0f)
        {
            nonlinearZoomScale = 1.0f;
        }
        var zoomDistance = Speed * Time.unscaledDeltaTime * nonlinearZoomScale;
        transform.Translate(new Vector3(0, 0, Input.mouseScrollDelta.y * zoomDistance));

        // Rotation around center of rotation
        if (Input.GetMouseButton(1))
        {
            var direction = previousPosition - Camera.main.ScreenToViewportPoint(Input.mousePosition);
            // Rotate camera in certain direction
            RotateAroundCenter(direction);
            previousPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        // Set center of rotation
        //if (Input.GetMouseButtonDown(2))
        //{
        //    if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        //    {
        //        return;
        //    }
        //    var meshCollider = hit.collider as MeshCollider;
        //    if (meshCollider == null || meshCollider.sharedMesh == null)
        //    {
        //        return;
        //    }

        //    // Focus camera on rotation center
        //    ChangeRotationCenter(hit.point);
        //}
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

        StartCoroutine(SmoothTranslation(newPosition));
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
