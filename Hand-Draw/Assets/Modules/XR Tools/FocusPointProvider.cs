using System.Collections;
using UnityEngine;

public class FocusPointProvider : MonoBehaviour
{
    // Public fields
    public float updatesPerSecond = 30f;
    public Transform headTransform;
    public Vector3 positionOffsetFromHead;
    public bool tiltAlongAxisZ = true;
    public bool tiltAlongAxisX = true;
    public bool trackHead = true;

    // Public properties for other scripts to access
    public Vector3 FocusPosition { get; private set; }
    public Quaternion FocusRotation { get; private set; }

    private Coroutine updateRoutine;

    private void OnEnable()
    {
        // Start updating the focus point
        updateRoutine = StartCoroutine(UpdateFocusPoint());
    }

    private void OnDisable()
    {
        // Stop updating the focus point
        if (updateRoutine != null)
            StopCoroutine(updateRoutine);
    }

    private IEnumerator UpdateFocusPoint()
    {
        float updateInterval = 1f / updatesPerSecond;
        yield return new WaitForSeconds(updateInterval);

        if (headTransform == null)
        {
            Debug.LogError("Head Transform is null on " + gameObject.name);
            trackHead = false;
        }

        // Calculate focus position
        Vector3 headFlatY = headTransform.forward;
        headFlatY.y = 0;
        headFlatY = headFlatY.normalized;
        FocusPosition = headTransform.position + (headFlatY * positionOffsetFromHead.z) + (headTransform.right * positionOffsetFromHead.x);
        FocusPosition = FocusPosition + Vector3.up * (positionOffsetFromHead.y - FocusPosition.y);

        // Calculate focus rotation
        Vector3 eulerAngles = Vector3.zero;
        eulerAngles.y = headTransform.eulerAngles.y;

        if (tiltAlongAxisZ)
            eulerAngles.z = headTransform.eulerAngles.z;
        if (tiltAlongAxisX)
            eulerAngles.x = headTransform.eulerAngles.x;
        FocusRotation = Quaternion.Euler(eulerAngles);

        transform.position = FocusPosition;
        transform.rotation = FocusRotation;

        if (trackHead)
            StartCoroutine(UpdateFocusPoint());
    }

    private void OnDrawGizmos()
    {
        // Visualize the focus point in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(FocusPosition, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(FocusPosition, FocusPosition + FocusRotation * Vector3.forward * 0.5f);

        // Draw a line perpendicular to the forward direction to visualize rotation more thoroughly
        Gizmos.color = Color.red;
        Gizmos.DrawLine(FocusPosition, FocusPosition + FocusRotation * Vector3.right * 0.5f);
    }
}
