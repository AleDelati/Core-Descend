using UnityEngine;

public class Elevator_Controller : MonoBehaviour {

    [SerializeField] float speed = 8;

    // private
    GameManager GM;
    Vector3 initialPos;

    int state;

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update() {
        state = GM.getState();
        input();
    }

    private void input() {
        if (Input.GetKey(KeyCode.UpArrow) && state == 1) {
            transform.position = transform.position + new Vector3(0, 1) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow) && state == 1) {
            transform.position = transform.position + new Vector3(0, -1) * speed * Time.deltaTime;
        }
    }

}
