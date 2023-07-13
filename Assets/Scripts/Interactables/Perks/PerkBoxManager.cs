using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PerkBoxManager : MonoBehaviour
{
    public int price;
    public Perk perkname;
    public bool perkBought;
    public Sprite perkIcon;

    private bool fading = false;
    private bool open;
    private bool pressedE = false;
    private bool purchasingPerk = false;

    private GameObject perkCopy;
    private Coroutine purchasingPerkCoroutine;

    public AudioClip openClip;
    public AudioClip[] tapeClips;
    public AudioClip closeClip;
    public AudioClip[] buyClips;

    private GameObject Player;
    private ScoreManager scoreManager;
    private TextMeshProUGUI textItem;
    public TextMeshProUGUI[] text;
    public GameObject[] UIperks;

    public Light2D focusLight;
    private float lightIntensity;
    public Light2D mist;
    private float mistIntensity;
    public ParticleSystem particles;

    public enum Perk {
        Juggernaut,
        StaminaUp,
        IncreasedDamage,
        ExtraPoints,
        QuickReload,
        QuickRegen,
        LuckyCrit
    };

    private void OnApplicationStart() {
        perkBought = false;
    }

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
            if (scoreManager.totalPoints >= price && !perkBought) {
                OpenCrate();
                
                if (pressedE && !purchasingPerk) {
                    PurchasePerk();
                    pressedE = false;
                }
            }
            else {
                CloseCrate();
            }
            if (pressedE && !purchasingPerk) {
                //notify player has perk   
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject == Player) {
            CloseCrate();
        }
    }


    private void PurchasePerk() {
        EquiptPerk();
        perkBought = true;
        scoreManager.IndicatePoints(-price);
        GetComponent<AudioSource>().PlayOneShot(buyClips[0]);
        GetComponent<AudioSource>().PlayOneShot(buyClips[1]);
        perkCopy = Instantiate(new GameObject());
        purchasingPerkCoroutine = StartCoroutine(PurchasingPerk(perkCopy));
        FadeOutText();
        PlayEffects(true);
    }

    private void EquiptPerk() {
        CloseCrate();
        Destroy(perkCopy);
        purchasingPerk = false;

        switch(perkname) {
            case Perk.Juggernaut :
                GetComponent<JuggernautManager>().CollectPerk();
                break;
            case Perk.StaminaUp :
                GetComponent<StaminaUpManager>().CollectPerk();
                break;
            case Perk.IncreasedDamage :
                GetComponent<IncreasedDamageManager>().CollectPerk();
                break;
            case Perk.ExtraPoints :
                GetComponent<ExtraPointsManager>().CollectPerk();
                break;
            case Perk.QuickReload :
                GetComponent<QuickReloadManager>().CollectPerk();
                break;
            case Perk.QuickRegen :
                GetComponent<QuickRegenManager>().CollectPerk();
                break;
            case Perk.LuckyCrit :
                //GetComponent<>().CollectPerk();
                break;
            default:
                break;
        }

        List<string> allPerks = Player.GetComponent<PlayerControllerPublic>().perks;
        allPerks.Add(gameObject.name);
        UIperks[allPerks.IndexOf(gameObject.name)].GetComponent<Image>().enabled = true;
        UIperks[allPerks.IndexOf(gameObject.name)].GetComponent<Image>().sprite = perkIcon;
    }

    private void OpenCrate() 
    {
        if (!open && !perkBought) {
            if (!fading && !purchasingPerk) { StartCoroutine(FadeInText()); }
            transform.GetComponent<Animator>().SetBool("Open", true);
            open = true;
            GetComponent<AudioSource>().PlayOneShot(openClip);
            GetComponent<AudioSource>().PlayOneShot(tapeClips[(int)Random.Range(0, tapeClips.Length)]);
        }
    }

    private void CloseCrate() 
    {
        if (open && !purchasingPerk) {
            transform.GetComponent<Animator>().SetBool("Open", false);
            StartCoroutine(FadeOutText());
            open = false;
            pressedE = false;
            GetComponent<AudioSource>().PlayOneShot(closeClip);
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

    IEnumerator PurchasingPerk(GameObject weapon)
    {   
        float transition = 3f;
        float effectDistance = 0.6f;
        purchasingPerk = true;
        Vector3 pos = new Vector2(transform.position.x, GetComponent<BoxCollider2D>().size.y / 2f);
        float time = 0;

        StartCoroutine(FadeOutText());
        perkCopy = new GameObject("Purchased Perk");
        perkCopy.transform.SetParent(transform);
        perkCopy.AddComponent<SpriteRenderer>();
        perkCopy.GetComponent<SpriteRenderer>().sprite = perkIcon;
        perkCopy.transform.localScale = Vector3.one * 1.5f;
        
        while (time < 1) {
            pressedE = false;
            time += Time.deltaTime / transition;
            perkCopy.transform.position = Vector3.Lerp(pos, new Vector3(pos.x, pos.y + effectDistance, pos.z), time);

            if (time > 0.75f) {
                perkCopy.GetComponent<SpriteRenderer>().color = Color.Lerp(perkCopy.GetComponent<SpriteRenderer>().color, new Color(255f, 255f, 255f, 0f), Mathf.InverseLerp(0.75f, 1f, time));
            }
            yield return null;
        }
        pos = perkCopy.transform.position;
        time = 0;
        
        Destroy(perkCopy);
        purchasingPerk = false;
        fading = false;
        if (open) {
            CloseCrate();
        }
        PlayEffects(false);
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
