using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour {

    public GameObject objectPrefab;
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
    }
}
