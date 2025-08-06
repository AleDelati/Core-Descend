using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum recyclable_Types { Box, Barrel }

public class Recyclable : MonoBehaviour {

    //Editor Config
    [Header("Recyclable object config")]
    [Tooltip("Asigna si el objeto debe ser destruido al reciclarse o no")]
    [SerializeField] bool destroyOnRecycle = false;
    [Tooltip("Asigna los puntos de vida que tiene el objeto si esta esta marcado como que debe destruirse al ser reciclado")]
    [SerializeField] float hitPoints;
    [Tooltip("Asigna el tipo de objeto reciclable para los reciclables no destruibles")]
    [SerializeField] recyclable_Types r_type;
    [Tooltip("Asigna los sprites que usan los diferentes tipos de reciclables si esta marcado como no destruible")]
    [SerializeField] Sprite[] boxSprites;
    [SerializeField] Sprite[] barrelSprites;

    [Header("Recyclable resources")]
    [SerializeField] int scrapMetalAmount;

    private float currentHitPoints;
    private bool recycled = false;
    private float disableIndicatorTime;

    Resource_Manager RM;
    SpriteRenderer SR;

    Light2D interacIndicator;

    // ----------------------------------------------------------------------------------------------------
    private void Awake() {
        RM = GameObject.Find("Game Manager").GetComponent<Resource_Manager>();
        SR = gameObject.GetComponent<SpriteRenderer>();

        interacIndicator = gameObject.GetComponent<Light2D>();
    }

    private void Start() {
        currentHitPoints = hitPoints;
        if(!destroyOnRecycle) { UpdateSprites(); } else { interacIndicator.lightCookieSprite = SR.sprite; }
    }

    private void Update() {
        DisableIndicator();
    }
    // ----------------------------------------------------------------------------------------------------

    //
    public void Recycle(float damage) {
        if (destroyOnRecycle) {
            currentHitPoints -= damage * Time.deltaTime;
            Debug.Log("Mining block, remaining hitpoints: " + currentHitPoints);
            if (currentHitPoints <= 0) {
                Destroy(gameObject);
                RM.AddScrapMetal(scrapMetalAmount);
                
            }
        } else {
            if (!recycled) {
                Debug.Log("Recolectaste algunos recursos");
                RM.AddScrapMetal(scrapMetalAmount);
                recycled = true;
                UpdateSprites();
            }
        }

    }

    private void UpdateSprites() {
        if (!recycled) {
            switch (r_type) {
                case recyclable_Types.Box:
                    int randomBox = Random.Range(1, boxSprites.Length);
                    SR.sprite = boxSprites[randomBox];
                    break;
                case recyclable_Types.Barrel:
                    SR.sprite = barrelSprites[1];
                    break;

            }

            interacIndicator.lightCookieSprite = SR.sprite;

        } else {
            switch (r_type) {
                case recyclable_Types.Box:
                    SR.sprite = boxSprites[0];
                    break;
                case recyclable_Types.Barrel:
                    SR.sprite = barrelSprites[0];
                    break;
            }
            interacIndicator.enabled = false;
        }
    }

    private void DisableIndicator() {
        if(!recycled && Time.time >= disableIndicatorTime) { interacIndicator.enabled = false; }
        else { interacIndicator.enabled = false; }
    }

    public void EnableIndicator() {
        if (!recycled) {
            interacIndicator.enabled = true;
            disableIndicatorTime = Time.time + 0.1f;
        }
    }

}
