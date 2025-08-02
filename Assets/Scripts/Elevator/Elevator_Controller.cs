using System;
using UnityEngine;

public class Elevator_Controller : MonoBehaviour {

    // Editor Config
    [Tooltip("Velocidad del Elevador X=Up | Y=Down")]
    [SerializeField] Vector2 elevatorSpeed = new Vector2(3, 4);
    [Tooltip("Radio en el que las torretas enemigas seran alertadas si las torretas del elevador disparan")]
    [Range(0f, 50f)]
    [SerializeField] float alertRad = 45;

    [Header("Radio de deteccion del player (Para subir al elevador)")]
    [SerializeField] Vector3 proximityOffset;
    [SerializeField] float proximityRadius = 4;

    // Main Ref
    GameManager GM;
    GameObject Elevator_Energy;

    // Child Ref
    [SerializeField]Elevator_Turret[] Turrets;

    // Var
    Vector3 initialPos;
    int state, lastInput = 0;
    bool playerOnReach = false;
    bool w_Energy = false, w_Turrets = false;

    // Events
    public static event Action onElevatorTriggerAlarm;    

    //
    private void Awake() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        Elevator_Energy = GameObject.Find("Elevator | Energy");
    }

    private void Start() {
        initialPos = transform.position;
    }

    private void Update() {
        state = GM.GetState();
        if (state == 0) { lastInput = 0; }
        
        Input();
        Update_StatusCheck();
        Update_ProximityCheck();
    }

    private void Input() {
        if (w_Energy) {

            //Mouse
            if (UnityEngine.Input.GetMouseButtonDown(0)) {
                onElevatorTriggerAlarm?.Invoke();
            }
            if (UnityEngine.Input.GetMouseButton(0)) {
                for (int i = 0; i < Turrets.Length; i++) {
                    Turrets[i].Shoot();
                }
            }
            if (UnityEngine.Input.GetMouseButtonUp(0)) {
                onElevatorTriggerAlarm?.Invoke();
            }

            if (UnityEngine.Input.GetKey(KeyCode.W) && state == 1 && transform.position.y < initialPos.y) {
                transform.position = transform.position + new Vector3(0, 1) * elevatorSpeed.x * Time.deltaTime;
                lastInput = 1;
            }
            if (UnityEngine.Input.GetKey(KeyCode.S) && state == 1) {
                transform.position = transform.position + new Vector3(0, -1) * elevatorSpeed.y * Time.deltaTime;
                lastInput = -1;
            }
        }
    }

    private void OnDrawGizmosSelected() {
        // Radio de deteccion del jugador ( Para subir al elevador )
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + proximityOffset, proximityRadius);

        // Radio en el que si se disparan las torretas del elevador, se alertara a las torretas enemigas
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRad);
    }

    // Checks
    private void Update_ProximityCheck() {
        playerOnReach = CheckPlayerProximity();
    }

    private void Update_StatusCheck() {
        w_Energy = Elevator_Energy.GetComponent<Repairable>().GetRepairedStatus();
        w_Turrets = Elevator_Energy.GetComponent<Repairable>().GetRepairedStatus();
    }

    public bool GetTurretStatus() {
        return w_Turrets;
    }
    // ----------------------------------------------------------------------------------------------------

    // Player Proximity
    private bool CheckPlayerProximity() {
        foreach(Collider2D collider in Physics2D.OverlapCircleAll(transform.position + proximityOffset, proximityRadius)) {
            if (collider.CompareTag("Player") == true) {
                return true;
            }
        }
        return false;
    }

    public bool GetProximity() {
        return playerOnReach;
    }

    public int GetLastInput() {
        return lastInput;
    }
    // ----------------------------------------------------------------------------------------------------
}
