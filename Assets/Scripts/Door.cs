using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour {

    [SerializeField] bool open = false;

    [SerializeField] Sprite doorOpen;
    [SerializeField] Sprite doorClose;

    [SerializeField] float interactRad = 0.5f;

    //

    private SpriteRenderer SR;
    private BoxCollider2D BoxCol;
    private ShadowCaster2D shCast;

    private bool playerOnReach = false;

    private void Start() {
        SR = GetComponent<SpriteRenderer>();
        BoxCol = GetComponent<BoxCollider2D>();
        shCast = GetComponent<ShadowCaster2D>();
    }

    private void Update() {
        CheckPlayerInteraction();
        Sprites();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRad);
    }

    // 
    private void CheckPlayerInteraction() {
        playerOnReach = CheckPlayerProximity();
        if(playerOnReach && Input.GetKeyDown(KeyCode.E)) {
            open = !open;
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
        if (open) {
            SR.sprite = doorOpen;
            BoxCol.enabled = false;
            shCast.trimEdge = 0.50f;
        } else if (!open) {
            SR.sprite = doorClose; BoxCol.enabled = true;
            shCast.trimEdge = 0.1f;
        }
    }

}
