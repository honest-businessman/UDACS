using UnityEngine;

public class DroneDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.relativeVelocity.magnitude > 5f) GetComponent<SFXScript>().Explode();
    }
}
