using UnityEngine;

public enum SkillUnlock { Repair_Turrets }

public class Computer : MonoBehaviour {

    // Editor Config
    [Header("Computer Config")]
    [Tooltip("Asigna que habilidad del jugador se desbloquea al reparar la computadora")]
    [SerializeField] SkillUnlock skillUnlock;

    // Main Ref
    GameManager GM;
    UI_Manager UI_M;

    // Var
    private bool repaired = false;

    // ----------------------------------------------------------------------------------------------------

    //
    private void Awake() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        UI_M = GameObject.Find("Game Manager").GetComponent<UI_Manager>();
    }

    private void Start() {

    }

    private void Update() {
        if(!repaired) { CheckRepair(); }
    }
    // ----------------------------------------------------------------------------------------------------

    //  Al completarse las reparaciones desbloquea la habilidad del jugador correspondiente
    private void CheckRepair() {
        if (gameObject.GetComponent<Repairable>().GetRepairedStatus()) {
            repaired = true;

            switch (skillUnlock) {
                case 0:
                    GM.SetPlayerSkillStatus("canRepairTurrets", true);
                    UI_M.SetNotificationText("Ahora puedo reparar las torretas del elevador", 3);
                    break;
                default:
                    //
                    break;
            }
        }
    }
    // ----------------------------------------------------------------------------------------------------
}
