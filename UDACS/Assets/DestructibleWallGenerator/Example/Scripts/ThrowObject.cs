using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour {

    public GameObject objectPrefab;
    public float throwForce = 1000f;
    public float lifetime = 5f;

    void Update(){
		if(Input.GetKeyDown(KeyCode.LeftControl)){
			Throw();
		}
	}
	
	// Create a sphere and throw it
	void Throw(){
        if (objectPrefab == null)
        {
            Debug.LogWarning("No prefab assigned to ThrowObject!");
            return;
        }

        // Instantiate prefab in front of the player
        GameObject go = Instantiate(objectPrefab, transform.position + transform.forward, Quaternion.identity);

        // Add Rigidbody if prefab doesn’t have one
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = go.AddComponent<Rigidbody>();
        }

        // Apply forward force
        rb.AddForce(transform.forward * throwForce);

        // Optional: add destroyer or self-destruct after 5 seconds
        if (go.GetComponent<DWGDestroyer>() == null)
        {
            go.AddComponent<DWGDestroyer>();
        }

        Destroy(go, lifetime);
    }
}
