using UnityEngine;

public class MatchTransform : MonoBehaviour
{
    public Transform source;
    public Transform target;
    public float positionLagTime = 0.5f;
    public float rotationLagTime = 0.5f;

    private Vector3 positionVelocity;
    private float angularVelocity;

    private void Update()
    {
        if (source == null || target == null)
        {
            Debug.LogWarning("Source or Target is not set for MatchTransform on " + gameObject.name);
            return;
        }

        // Smoothly update position with lag
        target.position = Vector3.SmoothDamp(
            target.position,
            source.position,
            ref positionVelocity,
            positionLagTime
        );

        // Smoothly update rotation with lag
        target.rotation = SmoothDampQuaternion(
            target.rotation,
            source.rotation,
            ref angularVelocity,
            rotationLagTime
        );
    }

    private Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref float velocity, float smoothTime)
    {
        // Calculate the angular difference in degrees
        float angleDifference = Quaternion.Angle(current, target);

        // Smooth the angular velocity
        float smoothedAngle = Mathf.SmoothDampAngle(0, angleDifference, ref velocity, smoothTime);

        // Interpolate the rotation using the smoothed angle
        return Quaternion.Slerp(current, target, smoothedAngle / angleDifference);
    }
}