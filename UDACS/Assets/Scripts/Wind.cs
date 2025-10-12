using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Wind : MonoBehaviour
{
    public float maxWindStrength = 15f;
    public float windChangeInterval = 3f;
    public float turbulenceAmount = 3f;

    Rigidbody rigidBody;
    Vector3 currentWindDirection;
    float timeTillWindChange;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        GenerateWindDirection();
    }

    void FixedUpdate()
    {
        timeTillWindChange -= Time.fixedDeltaTime;
        if (timeTillWindChange <= 0) GenerateWindDirection();

        rigidBody.AddForce(currentWindDirection + Random.insideUnitSphere * turbulenceAmount, ForceMode.Force);
    }

    void GenerateWindDirection()
    {
        timeTillWindChange = windChangeInterval;

        currentWindDirection = Random.onUnitSphere * Random.Range(maxWindStrength * 0.1f, maxWindStrength);
    }
}