using System.Collections;
using UnityEngine;

public class SFXScript : MonoBehaviour
{
    public AudioClip explosionSound;
    public float fireDuration = 2f;
    public float fireSize = 3f;
    public float fuseTime = 5f;
    private bool hasExploded = false;
    private AudioSource audioSource;
    private bool isActivated = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ActivateGrenade()
    {
        if (!isActivated)
        {
            isActivated = true;
            StartCoroutine(FuseAndExplode());
        }
    }
    public void DroneCrash()
    {
        if (!isActivated)
        {
            isActivated = true;
            Explode();
        }
    }

    private IEnumerator FuseAndExplode()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 pos = transform.position;

        // Explosion sound
        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        // Spawn fire and extra effects
        StartCoroutine(SpawnFire(pos));
        StartCoroutine(SpawnExplosionEffect(pos));

        Destroy(gameObject, 0.5f); // Destroy grenade object
    }

    private IEnumerator SpawnExplosionEffect(Vector3 position)
    {
        GameObject explosion = new GameObject("ExplosionEffect");
        explosion.transform.position = position;

        // Light effect
        Light light = explosion.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = Color.yellow;
        light.intensity = 8f;
        light.range = 8f;

        // Flash effect
        float fadeTime = 0.2f;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            light.intensity = Mathf.Lerp(8f, 0f, t / fadeTime);
            yield return null;
        }

        Destroy(explosion, 2f);
    }

    private IEnumerator SpawnFire(Vector3 position)
    {
        GameObject fire = new GameObject("FireEffect");
        fire.transform.position = position;

        ParticleSystem ps = fire.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, Color.red);
        main.startSize = new ParticleSystem.MinMaxCurve(0.5f, fireSize);
        main.startLifetime = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.loop = false;
        main.duration = fireDuration;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.5f;

        ps.Play();

        Light fireLight = fire.AddComponent<Light>();
        fireLight.color = Color.red;
        fireLight.intensity = 2f;
        fireLight.range = 5f;

        float timer = 0f;
        while (timer < fireDuration)
        {
            fireLight.intensity = Mathf.PingPong(timer * 5f, 2f);
            timer += Time.deltaTime;
            yield return null;
        }

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(fire);
    }
}
