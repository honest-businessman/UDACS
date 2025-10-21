using System.Collections;
using UnityEngine;

public class SFXScript : MonoBehaviour
{
    public bool isDrone = false;
    public AudioClip explosionSound;
    float fireDuration = 5f;
    public float fireSize = 5f;
    public bool hasExploded = false;
    private AudioSource audioSource;
    public Material fireMaterial;
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 pos = transform.position;

        // Explosion sound
        if (audioSource != null && explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        // Spawn fire and extra effects
        GameObject fireEffect = new GameObject("FireEffect");
        fireEffect.transform.position = pos;
        fireEffect.AddComponent<CoroutineRunner>().StartCoroutine(SpawnFire(pos, fireEffect));
        GameObject explosionEffect = new GameObject("ExplosionEffect");
        explosionEffect.transform.position = pos;
        explosionEffect.AddComponent<CoroutineRunner>().StartCoroutine(SpawnExplosionEffect(pos,explosionEffect));
    }

    private IEnumerator SpawnExplosionEffect(Vector3 position, GameObject explosion)
    {
        explosion.transform.position = position;

        explosion.transform.parent = null;
        // Light effect
        Light light = explosion.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = Color.yellow;

        light.intensity = isDrone ? 4f : 8f;
        light.range = isDrone ? 4f : 8f;

        // Flash effect
        float flashTime = 0.2f;
        float startIntensity = light.intensity;
        for (float t = 0; t < flashTime; t += Time.deltaTime)
        {
            light.intensity = Mathf.Lerp(startIntensity, 0f, t / flashTime);
            yield return null;
        }
        float fadeDuration = 5f;
        float initalIntensity = startIntensity;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            light.intensity = Mathf.Lerp(initalIntensity, 0f, t / fadeDuration);
            yield return null;
        }

        Destroy(explosion);
    }

    private IEnumerator SpawnFire(Vector3 position, GameObject fire)
    {
        fire.transform.position = position;
        var ps = fire.AddComponent<ParticleSystem>();

        var psRenderer = fire.GetComponent<ParticleSystemRenderer>();
        psRenderer.material = fireMaterial;
        psRenderer.renderMode = ParticleSystemRenderMode.Billboard;

        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.startLifetime = 5f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.5f, fireSize);
        main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, Color.red);

        ps.Play();

        fire.transform.parent = null;
        Light fireLight = fire.AddComponent<Light>();
        fireLight.color = Color.red;
        fireLight.intensity = isDrone ? 1f : 2f;
        fireLight.range = isDrone ? 3f : 5f;

        float baseIntensity = fireLight.intensity;
        float timer = 0f;

        while (timer < fireDuration)
        {
            fireLight.intensity = baseIntensity * 0.5f + Mathf.PingPong(timer * 2f, baseIntensity * 0.5f);
            timer += Time.deltaTime;
            yield return null;
        }

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        float remainDuration = Mathf.Max(0f,5f - fireDuration);
        yield return new WaitForSeconds(remainDuration);
        Destroy(fire);
    }
    internal class CoroutineRunner : MonoBehaviour { }
}