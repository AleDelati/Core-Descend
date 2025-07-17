using UnityEngine;

public class Elevator_Controller : MonoBehaviour {

    [SerializeField] float downSpeed = 4;
    [SerializeField] float upSpeed = 3;

    [SerializeField] Vector3 proximityOffset;
    [SerializeField] float proximityRadius = 4;

    // private
    GameManager GM;
    GameObject Elevator_Energy;

    Vector3 initialPos;

    int state, lastInput = 0;
    bool playerOnReach = false;

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        Elevator_Energy = GameObject.Find("Elevator | Energy");

        initialPos = transform.position;
    }

    private void Update() {
        state = GM.GetState();
        if (state == 0) { lastInput = 0; }
        
        Input();
        Update_ProximityCheck();
    }

    private void Input() {
        if (Elevator_Energy.GetComponent<Repairable>().GetRepairedStatus()) {
            if (UnityEngine.Input.GetKey(KeyCode.W) && state == 1 && transform.position.y < initialPos.y) {
                transform.position = transform.position + new Vector3(0, 1) * upSpeed * Time.deltaTime;
                lastInput = 1;
            }
            if (UnityEngine.Input.GetKey(KeyCode.S) && state == 1) {
                transform.position = transform.position + new Vector3(0, -1) * downSpeed * Time.deltaTime;
                lastInput = -1;
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + proximityOffset, proximityRadius);
    }

    private void Update_ProximityCheck() {
        playerOnReach = CheckPlayerProximity();
    }

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

}
