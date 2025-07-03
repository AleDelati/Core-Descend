using UnityEngine;
using UnityEngine.UIElements;

public class Elevator_Controller : MonoBehaviour {

    [SerializeField] float speed = 4;

    [SerializeField] Vector3 proximityOffset;
    [SerializeField] float proximityRadius = 4;

    // private
    GameManager GM;
    Vector3 initialPos;

    int state;
    bool playerOnReach = false;

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update() {
        state = GM.getState();
        
        input();
        UpdateProximityCheck();
    }

    private void input() {
        if (Input.GetKey(KeyCode.W) && state == 1) {
            transform.position = transform.position + new Vector3(0, 1) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) && state == 1) {
            transform.position = transform.position + new Vector3(0, -1) * speed * Time.deltaTime;
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

}
