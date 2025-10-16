using UnityEngine;
public class FollowDrone : MonoBehaviour
{
    public Transform droneTransform;
    public float altitude;
    void FixedUpdate()
    {
        transform.position = new Vector3(droneTransform.position.x, altitude, droneTransform.position.z);
        transform.eulerAngles = new Vector3(0, droneTransform.eulerAngles.y, 0);
    }
}