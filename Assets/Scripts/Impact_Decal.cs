using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Impact_Decal : MonoBehaviour {

    // Editor Config
    [Header("General Config")]
    [SerializeField] float lifeTime = 10.0f;
    [SerializeField] Sprite[] sprites;

    private float startTime;

    // Self Ref
    SpriteRenderer SR;
    Light2D _light;

    private void Awake() {
        SR = GetComponent<SpriteRenderer>();
        _light = GetComponent<Light2D>();
    }

    private void Start() {
        Destroy(gameObject, lifeTime);
        
        startTime = Time.time;
        SR.sprite = sprites[0];
    }

    private void Update() {
        UpdateSprites();
    }

    private void UpdateSprites() {
        if(Time.time >= startTime + 1 && Time.time <= startTime + 3) {
            SR.sprite = sprites[1];
            _light.intensity = 20;
        }else if(Time.time >= startTime + 3 && Time.time <= startTime + 6) {
            SR.sprite = sprites[2];
            _light.intensity = 10;
        } else if (Time.time >= startTime + 6 && Time.time <= startTime + 8) {
            SR.sprite = sprites[3];
            _light.intensity = 5;
        } else if (Time.time >= startTime + 8) {
            SR.sprite = sprites[4];
            _light.intensity = 2.5f;
        }
    }

    public void FlipSprite() {
        SR.flipX = !SR.flipX;
    }

}
