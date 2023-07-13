using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class GunBoxManager : MonoBehaviour
{
    public int price;
    private bool fading = false;
    private bool open;
    private bool pressedE = false;
    private bool purchasingWeapon = false;
    public float weaponCollectTime;

    private GameObject newWeapon;
    private GameObject newWeaponCopy;
    private Coroutine purchasingWeaponCoroutine;

    public AudioClip[] openClips;
    public AudioClip[] closeClips;
    public AudioClip[] buyClips;

    private GameObject Player;
    private ScoreManager scoreManager;
    private TextMeshProUGUI textItem;
    public TextMeshProUGUI[] text;
    public GameObject[] weapons;

    public Light2D focusLight;
    private float lightIntensity;
    public Light2D mist;
    private float mistIntensity;
    public ParticleSystem particles;


    void Start()
    {
        Player = GameObject.Find("Player");
        scoreManager = GameObject.Find("Points").GetComponent<ScoreManager>();
        textItem = text[0];
        foreach (TextMeshProUGUI textMesh in text) {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0f);
        }
        PlayEffects(false);
        lightIntensity = focusLight.intensity;
        mistIntensity = mist.intensity;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.E) && open) { pressedE = true; }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == Player && scoreManager.totalPoints >= price && !open) {
            OpenCrate();
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    { 
        if (other.gameObject == Player) {
            if (scoreManager.totalPoints >= price) {
                OpenCrate();
                
                if (pressedE && !purchasingWeapon) {
                    PurchaseWeapon();
                    pressedE = false;
                }
            }
            else {
                CloseCrate();
            }
        }
        if (purchasingWeapon && pressedE) {
            EquiptWeapon();
            pressedE = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject == Player) {
            CloseCrate();
            pressedE = false;
        }
    }


    private void PurchaseWeapon() {
        scoreManager.IndicatePoints(-price);
        GetComponent<AudioSource>().PlayOneShot(buyClips[0]);
        GetComponent<AudioSource>().PlayOneShot(buyClips[1]);
        newWeapon = Instantiate(weapons[Random.Range(0, weapons.Length)], transform);
        purchasingWeaponCoroutine = StartCoroutine(PurchasingWeapon(newWeapon));
        FadeOutText();
        PlayEffects(true);
    }

    private void EquiptWeapon() {
        StopCoroutine(purchasingWeaponCoroutine);
        CloseCrate();
        Destroy(newWeaponCopy);
        purchasingWeapon = false;
        Weapon_Script ws = newWeapon.GetComponent<Weapon_Script>();
        PlayerControllerPublic pcp = Player.GetComponent<PlayerControllerPublic>();
        
        if (ws.weaponType == Weapon_Script.WeaponType.primary) {
            if (pcp.inv[2] != null) {
                Destroy(pcp.inv[2]);
            }
            if (pcp.currEqupt == 2) { newWeapon.SetActive(true); }
            pcp.inv[2] = newWeapon;
        }
        else if (ws.weaponType == Weapon_Script.WeaponType.secondary) {
            if (pcp.inv[1] != null) {
                Destroy(pcp.inv[1]);
            }
            if (pcp.currEqupt == 1) { newWeapon.SetActive(true); }
            pcp.inv[1] = newWeapon;
        }
        newWeapon.name = newWeapon.name.Split('(')[0];
    }

    private void OpenCrate() 
    {
        if (!open) {
            if (!fading && !purchasingWeapon) { StartCoroutine(FadeInText()); }
            transform.GetChild(0).GetComponent<Animator>().SetBool("Open", true);
            open = true;
            GetComponent<AudioSource>().PlayOneShot(openClips[(int)Random.Range(0, openClips.Length)]);
        }
    }

    private void CloseCrate() 
    {
        if (open && !purchasingWeapon) {
            transform.GetChild(0).GetComponent<Animator>().SetBool("Open", false);
            StartCoroutine(FadeOutText());
            open = false;
            pressedE = false;
            GetComponent<AudioSource>().PlayOneShot(closeClips[(int)Random.Range(0, closeClips.Length)]);
            PlayEffects(false);
        }
    }

    private void PlayEffects(bool play) {
        if (play) {
            particles.Play();
            StartCoroutine(FadeEffects(focusLight, lightIntensity));
            StartCoroutine(FadeEffects(mist, mistIntensity));
        }
        else {
            particles.Stop();
            StartCoroutine(FadeEffects(focusLight, 0f));
            StartCoroutine(FadeEffects(mist, 0f));
        }
        IEnumerator FadeEffects(Light2D source, float newIntensity)
        {
            float initialIntensity = source.intensity;
            float time = 0f;

            while (time < 1f)
            {
                float t = time / 1f;
                float currentIntensity = Mathf.Lerp(initialIntensity, newIntensity, t);
                source.intensity = currentIntensity;

                time += Time.deltaTime;
                yield return null;
            }

            source.intensity = newIntensity;
        }
    }

    IEnumerator PurchasingWeapon(GameObject weapon)
    {   
        float transition = 2f;
        float effectDistance = 0.6f;
        purchasingWeapon = true;
        Vector3 pos = transform.position;
        float time = 0;
        float changeWeaponInterval = 0.05f;

        StartCoroutine(FadeOutText());
        newWeaponCopy = new GameObject("Purchased Weapon");
        newWeaponCopy.transform.SetParent(transform);
        newWeaponCopy.transform.localPosition = Vector3.zero;
        newWeaponCopy.AddComponent<SpriteRenderer>();
        newWeaponCopy.transform.localScale = Vector3.one * 2.5f;
        
        while (time < 1) {
            pressedE = false;
            time += Time.deltaTime / transition;
            newWeaponCopy.transform.position = Vector3.Lerp(pos, new Vector3(pos.x, pos.y + effectDistance, pos.z), time);
            if (time >= changeWeaponInterval) {        
                newWeaponCopy.GetComponent<SpriteRenderer>().sprite = weapons[Random.Range(0, weapons.Length)].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                changeWeaponInterval += 0.05f;
            }
            yield return null;
        }
        newWeaponCopy.GetComponent<SpriteRenderer>().sprite = weapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        pos = newWeaponCopy.transform.position;
        time = 0;
        
        while (time < 1) {
            time += Time.deltaTime / weaponCollectTime;
            newWeaponCopy.transform.position = pos + new Vector3(0f, Mathf.Sin(time * 20f) * (effectDistance / 10f), 0f);
            yield return null;
        }
        time = 0;

        while (time < 1) {
            time += Time.deltaTime;
            newWeaponCopy.GetComponent<SpriteRenderer>().color = Color.Lerp(newWeaponCopy.GetComponent<SpriteRenderer>().color, new Color(255f, 255f, 255f, 0f), time);
            yield return null;
        }
        
        Destroy(newWeaponCopy);
        Destroy(newWeapon);
        purchasingWeapon = false;
        fading = false;
        if (open) {
            CloseCrate();
        }
    }

    private IEnumerator FadeInText()
    {
        fading = true;
        Color originalColor = textItem.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;
            foreach (TextMeshProUGUI textMesh in text) {
                textMesh.color = Color.Lerp(originalColor, targetColor, t);
            }
            yield return null;
        }

        foreach (TextMeshProUGUI textMesh in text) {
            textMesh.color = targetColor;
        }
        fading = false;
    }

    private IEnumerator FadeOutText()
    {
        fading = true;
        Color originalColor = textItem.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;
            foreach (TextMeshProUGUI textMesh in text) {
                textMesh.color = Color.Lerp(originalColor, targetColor, t);
            }
            yield return null;
        }

        
        foreach (TextMeshProUGUI textMesh in text) {
            textMesh.color = targetColor;
        }
        fading = false;
    }
}
