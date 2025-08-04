using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour {

    // Editor Config
    [Header("General Config")]
    [SerializeField] bool open = false;
    [SerializeField] float interactRad = 1.5f;


    [Header("Sprites")]
    [SerializeField] Sprite[] doorSprites;
    [SerializeField] Sprite[] consoleSprites;

    // References
    private SpriteRenderer SR;
    private BoxCollider2D BoxCol;
    private ShadowCaster2D shCast;
    private Repairable Rep;
    private Light2D interactIndicator;

    // Ext Ref

    // Var
    private bool playerOnReach = false;
    public bool repairable;
    public bool repaired;
    
    //
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRad);
    }

    private void Start() {
        SR = GetComponent<SpriteRenderer>();
        BoxCol = GetComponent<BoxCollider2D>();
        shCast = GetComponent<ShadowCaster2D>();
        Rep = GetComponent<Repairable>();
        interactIndicator = transform.Find("Interact Indicator Light").GetComponent<Light2D>();

        repairable = Rep.GetRepairable();
        repaired = Rep.GetRepairedStatus();
        
        if(open) { shCast.trimEdge = 0.50f; }

    }

    private void Update() {
        CheckRepairs();
        CheckPlayerInteraction();
        Sprites();
    }

    // 
    private void CheckPlayerInteraction() {
        if (repaired && repairable) {
            playerOnReach = CheckPlayerProximity();
            if (playerOnReach && Input.GetKeyDown(KeyCode.E)) {
                open = !open;
            }
        }

        //  Controla el indicador de interaccion
        if (repaired) {
            interactIndicator.lightCookieSprite = SR.sprite;
            if(playerOnReach){ interactIndicator.enabled = true; }
            else { interactIndicator.enabled = false; }
        }
    }

    private bool CheckPlayerProximity() {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, interactRad)) {
            if (collider.CompareTag("Player") == true) {
                return true;
            }
        }
        return false;
    }

    private void Sprites() {
        // Door
        if (open) {
            SR.sprite = doorSprites[1];
            BoxCol.enabled = false;
            shCast.trimEdge = 0.50f;
        } else if (!open) {
            SR.sprite = doorSprites[0];
            BoxCol.enabled = true;
            shCast.trimEdge = 0.1f;
        }

        // Console
        if (!repairable) {
            transform.Find("Door Console - L").GetComponent<SpriteRenderer>().sprite = consoleSprites[0];
            transform.Find("Door Console - R").GetComponent<SpriteRenderer>().sprite = consoleSprites[1];
        } else if (repairable && !repaired){
            transform.Find("Door Console - L").GetComponent<SpriteRenderer>().sprite = consoleSprites[2];
            transform.Find("Door Console - R").GetComponent<SpriteRenderer>().sprite = consoleSprites[3];
        } else if (repairable && repaired) {
            transform.Find("Door Console - L").GetComponent<SpriteRenderer>().sprite = consoleSprites[4];
            transform.Find("Door Console - R").GetComponent<SpriteRenderer>().sprite = consoleSprites[5];
        }
    }

    // Aux

    private void CheckRepairs() {
        if (!repairable) { repairable = Rep.GetRepairable(); }
        if (!repaired && repairable) { repaired = Rep.GetRepairedStatus(); }
    }

}
