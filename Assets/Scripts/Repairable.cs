using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Repairable : MonoBehaviour {

    // Editor Config
    [Header("Debug Options")]
    [SerializeField] bool debug_Repair = false;

    [Header("General Config")]
    [SerializeField] float interactRad = 1.5f;
    [SerializeField] Vector3 interactOffset = Vector3.zero;

    [Header("Repair Config")]
    [SerializeField] bool repairable = false;
    [SerializeField] bool repaired = false;

    [SerializeField] bool repairIndicator = false;

    [SerializeField] bool fixSprites = false;
    [SerializeField] Sprite[] repairSprites;

    [Header("Repair resources")]
    [SerializeField] int repair_Scrap;

    [Header("Repair Objectives")]
    [Tooltip("Activa la posibilidad de reparacion de los objetos asignados")]
    [SerializeField] Repairable[] repairTargets;
    [Tooltip("Repara las luces asignadas")]
    [SerializeField] Light2D[] repairLights;
    [Tooltip("Repara las torretas del elevador asignadas")]
    [SerializeField] Elevator_Turret[] repairTurrets;

    // Main Ref
    Resource_Manager RM;
    SpriteRenderer SR;
    Light2D repairIndicatorLight;

    //Var
    private bool playerOnReach = false;
    // ----------------------------------------------------------------------------------------------------

    //
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + interactOffset, interactRad);
    }

    private void Awake() {
        RM = GameObject.Find("Game Manager").GetComponent<Resource_Manager>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if (repairIndicator) {
            repairIndicatorLight = transform.Find("Repair Indicator Light").GetComponent<Light2D>();
            repairIndicatorLight.lightCookieSprite = SR.sprite;
        }
        if(fixSprites){ SR.sprite = repairSprites[0]; }
    }

    private void Update() {
            if (!repaired && repairable) { CheckPlayerInteraction(); }
            if (!repaired && debug_Repair) { Repair(); }
    }
    // ----------------------------------------------------------------------------------------------------

    // 
    private void CheckPlayerInteraction() {
        playerOnReach = CheckPlayerProximity();

        // Controla la interaccion y reparacion
        if (playerOnReach) {

            if (playerOnReach && Input.GetKeyDown(KeyCode.E)) {
                if (RM.ScrapMetal() >= repair_Scrap) {
                    RM.SubtractScrapMetal(repair_Scrap);
                    Repair();
                } else {
                    Debug.Log("No tienes suficiente chatarra para reparar");
                }
            }
        }
        // Enciende o apaga el indicador de interaccion de reparacion dependiendo de si el jugador se acerca o se aleja
        if (repairIndicator && !repaired) {
            if (playerOnReach) { repairIndicatorLight.enabled = true; } else if (!playerOnReach) { repairIndicatorLight.enabled = false; }
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

    private void Repair() {
        repaired = true;

        if (fixSprites) { SR.sprite = repairSprites[1]; }

        // Si hay objetivos de reparacion, activa la posibilidad de repararlos
        for (int i = 0; i < repairTargets.Length; i++) {
            if (repairTargets[i] != null) { repairTargets[i].SetRepairable(true); }
        }
        // Si hay luces vinculadas a la reparacion las activa
        for (int i = 0; i < repairLights.Length; i++) {
            if (repairLights[i] != null) { repairLights[i].enabled = true; }
        }
        // Si hay torretas del elevador vinculadas las repara
        for (int i = 0; i < repairTurrets.Length; i++) {
            if (repairTurrets[i] != null) { repairTurrets[i].GetComponent<Elevator_Turret>().EnableTurret(); }
        }


        Debug.Log("Reparaciones Completadas");

        //  Apaga el indicador de interaccion de reparacion al completar las reparaciones
        if (repairIndicator && repaired) { repairIndicatorLight.enabled = false; }
    }

    //
    public void SetRepaired(bool status) {
        repaired = status;
        if (fixSprites) {
            if(status == true) { SR.sprite = repairSprites[1]; }
            else{ SR.sprite = repairSprites[0]; }
        }
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
    // ----------------------------------------------------------------------------------------------------
}
