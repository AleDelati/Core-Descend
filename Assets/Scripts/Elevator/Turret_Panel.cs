using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Turret_Panel : MonoBehaviour {

    //
    [Header("Turret Panel Config")]
    [SerializeField] float interactRad = 1.5f;

    [Header("Repair Config")]
    [SerializeField] bool repaired = false;
    [SerializeField] Sprite[] turretPanel_Sprites;
    [Tooltip("Repara las torretas del elevador asignadas")]
    [SerializeField] Elevator_Turret[] repairTurrets;
    [Header("Repair Costs")]
    [SerializeField] int repair_Scrap = 0;

    // Main Ref
    GameManager GM;
    UI_Manager UI_M;
    Resource_Manager RM;
    SpriteRenderer SR;
    Light2D repairIndicatorLight;

    // Var
    private bool playerOnReach = false;
    // ----------------------------------------------------------------------------------------------------

    //
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRad);
    }

    private void Awake() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        UI_M = GameObject.Find("Game Manager").GetComponent<UI_Manager>();
        RM = GameObject.Find("Game Manager").GetComponent<Resource_Manager>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        repairIndicatorLight = transform.Find("Repair Indicator Light").GetComponent<Light2D>();
        repairIndicatorLight.lightCookieSprite = SR.sprite;
        SR.sprite = turretPanel_Sprites[0];
    }

    private void Update() {
        if (!repaired) { CheckPlayerInteraction(); }
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
                    UI_M.SetNotificationText("No tengo suficiente chatarra para reparar", 1);
                }
            }
        }
        // Enciende o apaga el indicador de interaccion de reparacion dependiendo de si el jugador se acerca o se aleja
        if (!repaired) {
            if (playerOnReach) { repairIndicatorLight.enabled = true; } else if (!playerOnReach) { repairIndicatorLight.enabled = false; }
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

    private void Repair() {

        if (GM.GetPlayerSkillStatus("canRepairTurrets")) {
            repaired = true;
            SR.sprite = turretPanel_Sprites[1];

            for (int i = 0; i < repairTurrets.Length; i++) {
                if (repairTurrets[i] != null) { repairTurrets[i].GetComponent<Elevator_Turret>().EnableTurret(); }
            }

            UI_M.SetNotificationText("Torreta reparada", 1);
        } else {
            UI_M.SetNotificationText("Aun no se como reparar torretas", 2);
        }

        //  Apaga el indicador de interaccion de reparacion al completar las reparaciones
        if (repaired) { repairIndicatorLight.enabled = false; }
    }
    // ----------------------------------------------------------------------------------------------------


    //
    public void SetRepaired(bool status) {
        repaired = status;
        if (status == true) { SR.sprite = turretPanel_Sprites[1]; } else {
            SR.sprite = turretPanel_Sprites[0];
        }
    }

    public bool GetRepairedStatus() {
        return repaired;
    }
    // ----------------------------------------------------------------------------------------------------
}
