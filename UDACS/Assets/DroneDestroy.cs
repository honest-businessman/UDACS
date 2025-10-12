using UnityEngine;

public class DroneDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.relativeVelocity.magnitude > 10f)
        {
            GetComponent<SFXScript>().hasExploded = false;
            GetComponent<SFXScript>().Explode();
            GetComponent<DroneController>().droneOn = false;
        }
    }
}
