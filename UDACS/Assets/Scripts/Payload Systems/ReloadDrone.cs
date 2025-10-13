using UnityEngine;

public class ReloadDrone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ThrowObject reloadObject;
        if (reloadObject = other.gameObject.GetComponent<ThrowObject>()) reloadObject.loaded = true;
    }
}
