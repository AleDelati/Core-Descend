using UnityEngine;

public class Character_Controller : MonoBehaviour {

    [SerializeField] float speed = 8;

    // private
    
    GameManager GM;
    Elevator_Controller elevatorC;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    SpriteRenderer sR;

    //

    Vector3 initialPos;
    int state = 0;  // 0 - Normal | 1 - Elevador

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        elevatorC = GameObject.Find("Elevator").GetComponent<Elevator_Controller>();


        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sR = GetComponent<SpriteRenderer>();

        
        initialPos = transform.position;
    }

    private void Update() {
        input();
    }

    private void input() {
        if (Input.GetKey(KeyCode.A) && state == 0) {
            transform.position = transform.position + new Vector3(-1, 0) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) && state == 0) {
            transform.position = transform.position + new Vector3(1, 0) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W) && state == 0) {
            transform.position = transform.position + new Vector3(0, 1) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) && state == 0) {
            transform.position = transform.position + new Vector3(0, -1) * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.E) && elevatorC.GetProximity()) {
            EnterElevator();
        }
        if (Input.GetKeyDown(KeyCode.R) && state == 0) {
            transform.position = initialPos;
        }

        if (state == 1){ transform.position = elevatorC.transform.position + new Vector3(0, 5, 0); }
    }

    private void EnterElevator() {
        if (state == 0) { state = 1; sR.enabled = false; }
        else if(state == 1) { state = 0; sR.enabled = true; }
        GM.SetState(state);
        Debug.Log("Current state " + state);
    }

}
