using UnityEngine;

public class GrenadeExplode : MonoBehaviour
{

	public float radius = 10f;
	public float checkRadius = 12f;
	public float checkHeight = 100f;
	public float force = 200f;

	SFXScript sfx;
	void Start()
	{
        sfx = GetComponent<SFXScript>();
		StartCoroutine(Fuse());
	}
	
	private System.Collections.IEnumerator Fuse()
	{
		yield return new WaitForSeconds(5f);
        ExplodeForce();
		Destroy(gameObject);
    }
	
	// Explode force by radius only if a destructible tag is found
	void ExplodeForce()
	{
		sfx.Explode();
        Vector3 explodePos = transform.position;

        float step = radius / 2f;
        int steps = Mathf.CeilToInt(checkHeight / step);

		for (int i = 0; i < steps; i++)
		{
			Vector3 offset = Vector3.up * (-checkHeight / 2 + i * step);
			Collider[] colliders = Physics.OverlapSphere(explodePos + offset, checkRadius); 
			foreach (Collider hit in colliders) if (hit.GetComponent<Rigidbody>()) hit.GetComponent<Rigidbody>().isKinematic = false;
		}

        Collider[] hitColliders = Physics.OverlapSphere(explodePos, radius);
        foreach (Collider hit in hitColliders) if (hit.GetComponent<Rigidbody>()) hit.GetComponent<Rigidbody>().AddExplosionForce(force, explodePos, radius);
    }
}
