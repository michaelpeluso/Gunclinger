using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class AmmoBoxManager : MonoBehaviour
{public int price;
    private bool fading = false;
    private bool open;
    private bool pressedE = false;
    private bool purchasingAmmo = false;
    public float weaponCollectTime;

    private Coroutine purchasingAmmoCoroutine;

    public AudioClip[] openClips;
    public AudioClip[] closeClips;
    public AudioClip[] buyClips;

    private GameObject Player;
    private ScoreManager scoreManager;
    private TextMeshProUGUI textItem;
    public TextMeshProUGUI[] text;

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
                
                if (pressedE && !purchasingAmmo) {
                    PurchaseAmmo();
                    pressedE = false;
                }
            }
            else {
                CloseCrate();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject == Player) {
            CloseCrate();
            pressedE = false;
        }
    }

    private void OpenCrate() 
    {
        if (!open) {
            if (!fading && !purchasingAmmo) { StartCoroutine(FadeInText()); }
            transform.GetChild(0).GetComponent<Animator>().SetBool("Open", true);
            open = true;
            GetComponent<AudioSource>().PlayOneShot(openClips[(int)Random.Range(0, openClips.Length)]);
        }
    }

    private void CloseCrate() 
    {
        if (open && !purchasingAmmo) {
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
    
    private void PurchaseAmmo() {
        PlayerControllerPublic pcp = Player.GetComponent<PlayerControllerPublic>();
        GameObject currWeapon = pcp.inv[pcp.currEqupt];

        Weapon_Script ws = currWeapon.GetComponent<Weapon_Script>();
        ws.totalBulletsLeft = ws.maxClips * ws.magCapacity;

        scoreManager.IndicatePoints(-price);
        GetComponent<AudioSource>().PlayOneShot(buyClips[0]);
        GetComponent<AudioSource>().PlayOneShot(buyClips[1]);
        purchasingAmmoCoroutine = StartCoroutine(PurchasingAmmo());
        FadeOutText();
        PlayEffects(true);
    }

    IEnumerator PurchasingAmmo()
    {   
        float transition = 2f;
        purchasingAmmo = true;
        float time = 0;

        StartCoroutine(FadeOutText());
        
        while (time < 1) {
            pressedE = false;
            time += Time.deltaTime / transition;
            yield return null;
        }
        
        purchasingAmmo = false;
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
