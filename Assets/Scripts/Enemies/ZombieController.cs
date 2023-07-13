using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZombieController : MonoBehaviour
{
    public float health;
    private float maxHealth;
    public float speed;
    public float attackDamage;
    public float attackCoolDown;
    private float coolDownTimer;
    public float dealDamageDelay;
    public float dealDamageTimer;
    public Vector2 range;

    [Space (15)]
    private GameObject Player;
    private PlayerControllerPublic pcp;
    private Vector3 playerPos;
    private SpriteRenderer sr;
    public BoxCollider2D bc;
    [SerializeField] private LayerMask playerLM;
    private Animator animator;
    private Rigidbody2D rb;

    [Space (15)]
    public GameObject damageIndicator;
    public GameObject deathIndicator;
    public GameObject heathIndicator;
    private GameObject pointsIndicator;
    public float scoreMultiplyer;
    public GameObject[] bloodEffects;
    private bool dead;

    [Space (15)]
    public AudioClip[] moanClips;
    public AudioClip[] goreClips;
    public AudioClip[] hitClips;
    public AudioClip[] missClips;
    public Vector2 timeBetweenMoans;
    public float maxVolume;
    public float maxDistanceToHear;

    /* SELF CALLED */
    
    void Start()
    {
        Player = GameObject.Find("Player");
        pcp = Player.GetComponent<PlayerControllerPublic>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        dead = false;
        speed *= Random.Range(0.75f, 1.25f);
        sr.color = new Color((int)Random.Range(200, 255), (int)Random.Range(200, 255), (int)Random.Range(200, 255));
        transform.localScale *= new Vector2(Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f));
        pointsIndicator = GameObject.Find("Points");
        maxHealth = health;
        scoreMultiplyer = 1f;

        StartCoroutine(PlayMoans());
    }

    void Update()
    {
        if (health > 0.0f) {
            FollowPlayer();
            Attack();
        }
        else {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (rb) {
            rb.velocity *= new Vector2(speed * Random.Range(0.0f, 2.0f), 1.0f);
        }
    }

    /* MOVEMENT */

    void FollowPlayer() 
    {
        playerPos = Player.transform.position;

        if (playerPos.x > transform.position.x) {
            sr.flipX = false;
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else {
            sr.flipX = true;
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }

        if (PlayerInSight()) {
            rb.velocity = Vector2.zero;
            animator.SetBool("run", false);
        }
        else {
            animator.SetBool("run", true);
        }

    }


    /* ACTIONS */

    void Attack() 
    {
        coolDownTimer += Time.deltaTime;

        if (PlayerInSight()) {
            if (coolDownTimer >= attackCoolDown) {
                coolDownTimer = 0.0f;
                animator.SetTrigger("attack");
            }
        }
    }

    void DamagePlayer() {
        if (PlayerInSight()) {
            GetComponent<AudioSource>().PlayOneShot(hitClips[(int)Random.Range(0, hitClips.Length)]);
            pcp.TakeDamage(attackDamage);
           
            int x = Mathf.FloorToInt(Random.Range(0.0f, 7.9f));
            BoxCollider2D pbc = Player.GetComponent<BoxCollider2D>();
            GameObject blood = Instantiate(bloodEffects[x], new Vector2(pbc.bounds.center.x, pbc.bounds.center.y), transform.rotation * Quaternion.Euler(0.0f, 0.0f, 180f));
            if (sr.flipX) {
                blood.GetComponent<SpriteRenderer>().flipX = true;
            }
            Destroy(blood, 0.2f);
        }
    }

    private bool PlayerInSight() 
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            bc.bounds.center,
            bc.bounds.size,
            0.0f,
            Vector2.left, 
            0.0f,
            playerLM
        );
        return  hit.collider != null;
    }

    public void TakeDamage(float damage) {
        health -= damage;
        if (health < 0) {
            health = 0;
        }

        heathIndicator.GetComponent<HealthBarManager>().UpdateHealthBar(health, maxHealth);
        pointsIndicator.GetComponent<ScoreManager>().IndicatePoints(damage);

        GameObject di = Instantiate(damageIndicator, gameObject.transform);
        di.GetComponent<DamageIndicatorManager>().IndicateDamage(gameObject, Mathf.Round(damage * scoreMultiplyer));
        
        GetComponent<AudioSource>().volume = 1f;
        GetComponent<AudioSource>().PlayOneShot(goreClips[(int)Random.Range(0, goreClips.Length)]);
    }
    
    public void Die() {
        if (health <= 0.0f && !dead) {
            animator.SetTrigger("death");
            rb.velocity /= 2.0f;
            dead = true;
            
            GameObject dei = Instantiate(deathIndicator, gameObject.transform);
            dei.GetComponent<DeathIndicatorManager>().IndicateDeath(gameObject);

            GetComponent<BoxCollider2D>().size = Vector3.zero;
            GetComponent<CircleCollider2D>().enabled = false;
            PlayerPrefs.SetInt("zombiesAlive", PlayerPrefs.GetInt("zombiesAlive") - 1);
            StopCoroutine("PlayMoans");
            GetComponent<AudioSource>().volume = 0f;
        }
    }

    public void DestroyMe() {
        rb.velocity = Vector2.zero;
        animator.speed = 0;
        Destroy(gameObject, 2f);
    }

    /* AUDIO */
    IEnumerator PlayMoans() {
        while (!dead) {
            float waitTime = Random.Range(timeBetweenMoans.x, timeBetweenMoans.y);
            yield return new WaitForSeconds(waitTime);

            float distance = Vector2.Distance(transform.position, Player.transform.position);
            float volume = Mathf.Lerp(maxVolume, 0f, distance / maxDistanceToHear);
            GetComponent<AudioSource>().volume = Mathf.Clamp01(volume);
            GetComponent<AudioSource>().PlayOneShot(moanClips[(int)Random.Range(0, moanClips.Length)]);
        }
    }
}
