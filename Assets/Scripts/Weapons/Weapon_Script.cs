using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class Weapon_Script : MonoBehaviour
{
    private PlayerControllerPublic pcpScript;

    [Space (15)]
    public WeaponType weaponType;
    public bool fullAuto;
    [SerializeField] private int magCapacity;
    public int magBulletsLeft;
    public int totalBulletsLeft;
    public int maxClips;

    private float lastFired;
    [SerializeField] private float fireRate; //bullets per second
    [SerializeField] [Range(0, 100)] private float damage;
    [SerializeField] private float bulletSpeed;
    public float reloadTime;

    [Space (15)]
    [Range(0, 0.01f)] public float recoil;
    private int shotsFired = 0;
    private float recoilCoolDown = 0.25f;
    private float duration = 0.2f; 
    private float magnitude;
    private bool isReloading;
    private Coroutine reloadCoroutine = null;

    [Space(15)]
    public GameObject bullet;
    public GameObject Player;
    private SpriteRenderer sr;
    public Sprite sprite;    
    public GameObject hands;
    public Sprite handSprite;
    public GameObject muzzle;
    private Vector3 mousePos;
    
    private Vector3 localPos = Vector3.zero;
    private bool spriteIsFlipped;
    private GameObject pivotAnchor;

    [Space(15)]
    public AudioClip[] fireClips;
    public AudioClip[] reloadClips;
    public AudioClip[] equiptClips;
    public AudioClip[] emptyClips;
    private bool start = false;

    [Space(15)]
    public GameObject primaryUI;
    public GameObject secondaryUI;

    private void OnDisable() {
        muzzle.GetComponent<SpriteRenderer>().sprite = null;
        shotsFired = 0;
        isReloading = false;
    }

    void Start()
    {
        gameObject.SetActive(false);
        transform.parent = GameObject.Find("Weapons").transform;
        Player = GameObject.Find("Player");
        sr = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        pivotAnchor = transform.parent.gameObject;
        pcpScript = Player.GetComponent<PlayerControllerPublic>();
        transform.localPosition = localPos;
        magnitude = recoil * 15f;
        magBulletsLeft = magCapacity;
        totalBulletsLeft = magCapacity * maxClips - 1;
        primaryUI = GameObject.Find("PrimaryUI");
        secondaryUI = GameObject.Find("SecondaryUI");

        if (weaponType == WeaponType.primary) {
            sr.sortingOrder = 6;
            hands.GetComponent<SpriteRenderer>().sortingOrder = 7;
        }
        else if (weaponType == WeaponType.secondary) {
            sr.sortingOrder = 4;
            hands.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }

    }

    private void OnEnable() {
        if (start) { gameObject.GetComponent<AudioSource>().PlayOneShot(equiptClips[(int)Random.Range(0, equiptClips.Length)]); }
        start = true;
    }

    void Update()
    {
        pcpScript = Player.GetComponent<PlayerControllerPublic>();
        spriteIsFlipped = Player.GetComponent<PlayerControllerPublic>().spriteIsFlipped;

        ConstantAim();
        Shoot();
        ManageRecoil();
        UpdateUI();
        Reload();
    }

    private void ConstantAim() 
    {
        mousePos = Input.mousePosition;
        mousePos.z = 0.0f;
 
        Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
 
        float anlge = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, anlge);
        
        if (pcpScript.spriteIsFlipped) {
            gameObject.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        }
        else { 
            gameObject.transform.localScale = Vector3.one;
        }
    }

    public enum WeaponType {
        primary,
        secondary
        //, knife
    }

    void ControlArms() 
    {
        SpriteRenderer handsSR =  hands.GetComponent<SpriteRenderer>();
        handsSR.sprite = handSprite;

        if (pcpScript.spriteIsFlipped) {
            handsSR.flipY = true;
        }
        else { 
            handsSR.flipY = false;
        }
    }

    void Shoot() 
    {
        if (Time.time - lastFired > 0.6f / fireRate && magBulletsLeft > 0) {
            if (fullAuto && Input.GetMouseButton(0) || Input.GetMouseButtonDown(0)) {
                ShootVisualFX();
                shotsFired++;
                magBulletsLeft--;

                GameObject bulletInstance = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);
                bulletInstance.GetComponent<BulletHandler>().speed = bulletSpeed;
                bulletInstance.GetComponent<BulletHandler>().damage = damage;
                bulletInstance.GetComponent<AudioSource>().PlayOneShot(fireClips[(int)Random.Range(0, fireClips.Length)]);

                Camera.main.GetComponent<CameraShake>().Shake(duration, magnitude * shotsFired);

                if (isReloading) {
                    isReloading = false;
                    GetComponent<AudioSource>().Stop();
                }

                lastFired = Time.time;
            }
        }
        else if (Input.GetMouseButtonDown(0)) {
            GetComponent<AudioSource>().PlayOneShot(emptyClips[(int)Random.Range(0, emptyClips.Length)]);
        }
    }

    void ShootVisualFX() {
        if (gameObject.name != "Nerf Strike") {
            muzzle.GetComponent<Animator>().SetTrigger("shoot");
            if (muzzle.transform.GetChild(0)) { muzzle.transform.GetChild(0).GetComponent<ParticleSystem>().Play(); }
            StartCoroutine(MuzzleFlash());
            StartCoroutine(ManageRecoil());
            RecoilManager();
        }
    }

    IEnumerator RecoilManager() {
        yield return new WaitForSeconds(recoilCoolDown);
    }

    IEnumerator MuzzleFlash() 
    {
        muzzle.GetComponent<Light2D>().enabled = true;
        muzzle.GetComponent<Light2D>().intensity = 10f;
        yield return new WaitForSeconds(0.015f);
        muzzle.GetComponent<Light2D>().intensity = 5f;
        yield return new WaitForSeconds(0.03f);
        muzzle.GetComponent<Light2D>().intensity = 2f;
        yield return new WaitForSeconds(0.03f);
        muzzle.GetComponent<Light2D>().enabled = false;
        yield return new WaitForSeconds(recoilCoolDown);
        shotsFired--;
    }

    IEnumerator ManageRecoil() 
    {
        float elapsed = 0f;
        float recoilAngle = recoil * 500f;
        float recoilIncrease = shotsFired;

        while (elapsed < 0.1f) {
            transform.localPosition = new Vector3(
                localPos.x + Random.Range(-recoil, recoil) * recoilIncrease, 
                localPos.y + Random.Range(-recoil, recoil) * recoilIncrease, 
                localPos.z
            );

            if (spriteIsFlipped) {
                transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + Random.Range(-recoilAngle, 0f) * recoilIncrease);
            }
            else {
                transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + Random.Range(0f, recoilAngle) * recoilIncrease);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = localPos;
    }

    void UpdateUI() 
    {
        if (weaponType == WeaponType.primary) {
            primaryUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameObject.name;
            primaryUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = totalBulletsLeft.ToString();
            primaryUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = magBulletsLeft.ToString();
            primaryUI.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else if (weaponType == WeaponType.secondary) {
            secondaryUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameObject.name;
            secondaryUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = totalBulletsLeft.ToString();
            secondaryUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = magBulletsLeft.ToString();
            secondaryUI.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    void Reload() 
    {
        if (Input.GetKey(KeyCode.R) && magBulletsLeft != magCapacity && totalBulletsLeft > 0 && !isReloading) {
            reloadCoroutine = StartCoroutine(Reloading(GetComponent<AudioSource>()));
        }
    }

    IEnumerator Reloading(AudioSource audioSource) 
    {
        audioSource.clip = reloadClips[(int)Random.Range(0, reloadClips.Length)];
        audioSource.Play();

        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        if (!isReloading) { yield break; }

        audioSource.clip = equiptClips[(int)Random.Range(0, equiptClips.Length)];
        audioSource.Play();

        int bulletsNeeded = magCapacity - magBulletsLeft;

        if (bulletsNeeded <= totalBulletsLeft)
        {
            magBulletsLeft += bulletsNeeded;
            totalBulletsLeft -= bulletsNeeded;
        }
        else
        {
            magBulletsLeft += totalBulletsLeft;
            totalBulletsLeft = 0;
        }
        
    }
}
