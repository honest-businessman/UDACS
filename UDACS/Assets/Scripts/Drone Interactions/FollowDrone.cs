using UnityEngine;
public class FollowDrone : MonoBehaviour
{
    public Transform droneTransform;
    public Transform followTransform;
    public float altitude;
    void FixedUpdate()
    {
        followTransform.position = new Vector3(droneTransform.position.x, altitude, droneTransform.position.z);
        followTransform.eulerAngles = new Vector3(0, droneTransform.eulerAngles.y, 0);
    }
}