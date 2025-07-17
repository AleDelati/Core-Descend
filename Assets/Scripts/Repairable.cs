using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Repairable : MonoBehaviour {

    [Header("General Config")]
    [SerializeField] float interactRad = 1.5f;
    [SerializeField] Vector3 interactOffset = Vector3.zero;

    [Header("Repair Config")]
    [SerializeField] bool repairable = false;
    [SerializeField] bool repaired = false;
    [SerializeField] int repair_Scrap;

    [SerializeField] bool fixSprites = false;
    [SerializeField] Sprite[] repairSprites;

    [SerializeField] Repairable[] repairTargets;
    [SerializeField] Light2D[] repairLights;

    Resource_Manager RM;
    SpriteRenderer SR;

    private bool playerOnReach = false;

    //
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + interactOffset, interactRad);
    }

    private void Start() {
        RM = GameObject.Find("Game Manager").GetComponent<Resource_Manager>();
        SR = GetComponent<SpriteRenderer>();

        if(fixSprites){ SR.sprite = repairSprites[0]; }
    }

    private void Update() {
            if (!repaired && repairable) { CheckPlayerInteraction(); }
    }

    // 
    private void CheckPlayerInteraction() {
        playerOnReach = CheckPlayerProximity();
        if (playerOnReach && Input.GetKeyDown(KeyCode.E)) {
            if(RM.ScrapMetal() >= repair_Scrap) {
                Debug.Log("Reparaciones Completadas");
                RM.SubtractScrapMetal(repair_Scrap);
                repaired = true;

                if (fixSprites){ SR.sprite = repairSprites[1]; }

                // Si hay objetivos de reparacion, activa la posibilidad de repararlos
                for (int i = 0; i < repairTargets.Length; i++) {
                    if(repairTargets[i] != null) { repairTargets[i].SetRepairable(true); }
                }
                // Si hay luces vinculadas a la reparacion las activa
                for (int i = 0; i < repairLights.Length; i++) {
                    if (repairLights[i] != null) { repairLights[i].enabled = true; }
                }

            } else {
                Debug.Log("No tienes suficiente chatarra para reparar");
            }
        }
    }

    private bool CheckPlayerProximity() {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + interactOffset, interactRad)) {
            if (collider.CompareTag("Player") == true) {
                return true;
            }
        }
        return false;
    }

    public bool GetRepairedStatus() {
        return repaired;
    }

    public void SetRepairable(bool status) {
        repairable = status;
    }

    public bool GetRepairable() {
        return repairable;
    }

}
