using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class DWGColliderEnabler : MonoBehaviour
{
	Rigidbody rb;
	void Start() => rb = GetComponent<Rigidbody>();

    void OnCollisionEnter(Collision col)
    {
		if (!rb.isKinematic) StartCoroutine(SetStatic());
    }

    private System.Collections.IEnumerator SetStatic()
    {
        yield return new WaitForSeconds(30f);
        rb.isKinematic = true;
    }
}
