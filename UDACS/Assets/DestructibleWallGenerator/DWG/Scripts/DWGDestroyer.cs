using UnityEngine;

public class DWGDestroyer : MonoBehaviour
{

	public float radius = 40;
	public float force = 100f;

	bool explodedOrdinance = false;

	SFXScript sfx;
	void Start()
	{
        sfx = GetComponent<SFXScript>();
	}
	
	void OnCollisionEnter(Collision col)
	{
        if (!explodedOrdinance)
        {
			explodedOrdinance = true;
			ExplodeForce();
        }
		Destroy(gameObject);
    }
	
	// Explode force by radius only if a destructible tag is found
	void ExplodeForce()
	{
		sfx.Explode();
        Vector3 explodePos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explodePos, radius); 
		foreach (Collider hit in colliders)
		{
			if (hit.GetComponent<Rigidbody>())
			{
				hit.GetComponent<Rigidbody>().isKinematic = false; 
				hit.GetComponent<Rigidbody>().AddExplosionForce(force, explodePos,radius);
			}
		}
	}
}
