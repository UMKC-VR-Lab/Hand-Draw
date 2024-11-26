using System.Collections;
using Oculus.Interaction;
using UnityEngine;

public class HandSculpt : MonoBehaviour
{
    public DensityManager densityManager;
    public Transform targetFinger, indexFinger, targetThumb;
    public GameObject thumbSphere, indexSphere, middleSphere;
    private float scale = 100.0f;
    public bool debugSpheres = false;
    public HandVisual handVisual;

    private void Awake()
    {
        StartCoroutine(WaitForFinger());
    }

    private IEnumerator WaitForFinger()
    {
        yield return new WaitForSeconds(1.0f);

        // Use GetTransformByHandJointId to retrieve specific finger tips
        targetThumb = handVisual.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandThumbTip);
        indexFinger = handVisual.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandIndexTip);
        targetFinger = handVisual.GetTransformByHandJointId(Oculus.Interaction.Input.HandJointId.HandMiddleTip);

        if (targetFinger == null || indexFinger == null || targetThumb == null)
            StartCoroutine(WaitForFinger());
    }

    private void FixedUpdate()
    {
        if (targetFinger == null || indexFinger == null || targetThumb == null || densityManager == null)
            return;

        CheckForPinches();
        if(debugSpheres)
        {
            thumbSphere.transform.position = targetThumb.transform.position;
            middleSphere.transform.position = targetFinger.transform.position;
            indexSphere.transform.position = indexFinger.transform.position;
        }
    }

    private void CheckForPinches()
    {
        // If pinching index to thumb
        if (Vector3.Distance(targetThumb.position, indexFinger.position) < 0.05f)
        {
            Vector3 midpoint = (targetThumb.position + indexFinger.position) / 2.0f;
            AddDensityWithBlur(0.5f, 0.1f, midpoint);
        }

        // If pinching middle finger to thumb
        if (Vector3.Distance(targetThumb.position, targetFinger.position) < 0.05f)
        {
            Vector3 midpoint = (targetThumb.position + targetFinger.position) / 2.0f;
            AddDensityWithBlur(0.5f, 0.1f, midpoint);
        }
    }

    private void AddDensityWithBlur(float amount, float radius, Vector3 pos)
    {
        int radiusInVoxels = Mathf.CeilToInt(radius * scale);
        int xCenter = Mathf.FloorToInt(pos.x * scale);
        int yCenter = Mathf.FloorToInt(pos.y * scale);
        int zCenter = Mathf.FloorToInt(pos.z * scale);

        for (int x = -radiusInVoxels; x <= radiusInVoxels; x++)
        {
            for (int y = -radiusInVoxels; y <= radiusInVoxels; y++)
            {
                for (int z = -radiusInVoxels; z <= radiusInVoxels; z++)
                {
                    float distance = Mathf.Sqrt(x * x + y * y + z * z) / scale;
                    if (distance <= radius)
                    {
                        float scaledAmount = amount * (1.0f - (distance / radius));
                        densityManager.AddToDensity(scaledAmount, xCenter + x, yCenter + y, zCenter + z);
                    }
                }
            }
        }
    }

    private void SetDensityForFinger(Vector3 fingerPosition)
    {
        // Scale and floor the position to grid coordinates
        int x = Mathf.FloorToInt(fingerPosition.x * scale);
        int y = Mathf.FloorToInt(fingerPosition.y * scale);
        int z = Mathf.FloorToInt(fingerPosition.z * scale);

        // Set density to 1 at the exact grid position of the finger tip
        densityManager.SetDensity(1.0f, x, y, z);
    }
}
