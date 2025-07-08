using UnityEngine;

public class Elevator_Controller : MonoBehaviour {

    [SerializeField] float speed = 4;

    [SerializeField] Vector3 proximityOffset;
    [SerializeField] float proximityRadius = 4;

    // private
    GameManager GM;
    Vector3 initialPos;

    int state, lastInput = 0;
    bool playerOnReach = false;

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update() {
        state = GM.GetState();
        if (state == 0) { lastInput = 0; }
        
        Input();
        UpdateProximityCheck();
    }

    private void Input() {
        if (UnityEngine.Input.GetKey(KeyCode.W) && state == 1) {
            transform.position = transform.position + new Vector3(0, 1) * speed * Time.deltaTime;
            lastInput = 1;
        }
        if (UnityEngine.Input.GetKey(KeyCode.S) && state == 1) {
            transform.position = transform.position + new Vector3(0, -1) * speed * Time.deltaTime;
            lastInput = -1;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + proximityOffset, proximityRadius);
    }

    private void UpdateProximityCheck() {
        playerOnReach = CheckProximity();
    }

    private bool CheckProximity() {
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
