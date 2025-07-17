using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FuseBox : MonoBehaviour {

    [SerializeField] Sprite[] fuseBoxSprites;
    [SerializeField] Sprite[] fuseBoxLightSprites;

    SpriteRenderer SR;
    Repairable Rep;
    Light2D _light;

    private bool repaired;

    //
    private void Start() {
        SR = GetComponent<SpriteRenderer>();
        Rep = GetComponent<Repairable>();
        _light = Rep.GetComponent<Light2D>();

        repaired = Rep.GetRepairedStatus();
    }

    private void Update() {
        CheckRepairs();
        Sprites();
    }

    private void CheckRepairs() {
        if(!repaired) { repaired = Rep.GetRepairedStatus(); }
    }

    private void Sprites() {
        if (!repaired) { SR.sprite = fuseBoxSprites[0]; _light.lightCookieSprite = fuseBoxLightSprites[0]; }
        else { SR.sprite = fuseBoxSprites[1]; _light.lightCookieSprite = fuseBoxLightSprites[1]; }
    }

}
