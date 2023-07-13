using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    
    [HideInInspector] public float damage;
    [HideInInspector] public bool damageFallOff = true;
    public float headshotMultiplyer;

    public float critChance;
    public float critBoost;
    private bool critHit;
    public GameObject critHitObject;

    public float lifetime;
    public float speed;
    public GameObject headshotObject;

    private Rigidbody2D rb;
    public GameObject[] bloodEffects;
    [HideInInspector] public float damageMultiplyer;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        transform.parent = GameObject.Find("BulletHolder").transform;
        Destroy(gameObject, lifetime);

        damage = Mathf.Round(Random.Range(damage * 0.9f, damage * 1.1f));
        if (damageMultiplyer != 0) {
            damage = (int)(damage * damageMultiplyer);
        }

        critHit = false;
        CalculateCrit();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 7 || other.gameObject.name == "Ground") {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            if (other.gameObject.layer == 9) {        
                
                if (other.GetType() == typeof(CircleCollider2D)) {
                    damage = Mathf.Round(damage * headshotMultiplyer);
                    other.GetComponent<ZombieController>().TakeDamage(damage);
                    HeadshotEffect(other.transform.position.x);
                }
                else {
                    other.GetComponent<ZombieController>().TakeDamage(damage);
                }

                BloodEffect();
            }
        }
    }

    private void BloodEffect() {
        int x = Mathf.FloorToInt(Random.Range(0.0f, 7.9f));
        GameObject blood = Instantiate(bloodEffects[x], transform.position, transform.rotation * Quaternion.Euler(0.0f, 0.0f, Random.Range(160f, 200f)));
        Destroy(blood, 0.25f);
    }

    private void HeadshotEffect(float x) {
        GameObject smoke = Instantiate(headshotObject, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        //smoke.transform.position = new Vector2(x, transform.position.y);
        Destroy(smoke, 0.2f);
    }

    private void Update() {
        if (!GetComponent<AudioSource>().isPlaying)
        {
            Destroy(gameObject);
        }
        if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), GetComponent<Renderer>().bounds)) {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        
        if (damageFallOff) {        
            damage -= Time.deltaTime * damage;
        }
    }

    void CalculateCrit() {
        if (critChance < Random.Range(0, 1)) {
            damage *= critBoost;
            critHit = true;
        }
    }
}
