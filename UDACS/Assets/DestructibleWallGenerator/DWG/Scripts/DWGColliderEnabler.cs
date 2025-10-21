using UnityEngine;

public class DWGColliderEnabler : MonoBehaviour
{
	Rigidbody rb;
	Collider col;
	void Start()
	{
		if (col = gameObject.GetComponent<Collider>())
		{ // If this game object has a collider, continue
			col.enabled = true; // Enable the collider
		}
		rb = GetComponent<Rigidbody>();
	}

    void OnCollisionEnter(Collision col)
    {
		if (!rb.isKinematic) Destroy(gameObject, 30f);
    }
}
