using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour {

    public GameObject objectPrefab;
    public float throwForce = 1000f;
    public float lifetime = 5f;
    public bool loaded = false;
    public bool requiresLoading = true;

    void Update()
    {
        if (PlayerInteraction.Deploy.triggered && loaded) Throw();
    }
	
	// Create a grenade and drop it
	void Throw()
    {
        if (!requiresLoading) loaded = false;
        GameObject go = Instantiate(objectPrefab, transform.position - transform.up, Quaternion.identity);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        
        SFXScript sfx = go.GetComponent<SFXScript>();
        if (sfx != null)
        {
            sfx.ActivateGrenade();
        }
        Destroy(go, lifetime);
    }

}
