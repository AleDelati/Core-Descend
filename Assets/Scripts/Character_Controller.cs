using UnityEngine;

public class Character_Controller : MonoBehaviour {

    [SerializeField] float speed = 8;

    // private
    GameManager GM;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    SpriteRenderer sR;

    Vector3 initialPos;
    int state = 0;  // 0 - Normal | 1 - Elevador

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();

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
        if (Input.GetKeyDown(KeyCode.E)) {
            EnterElevator();
        }
        if (Input.GetKeyDown(KeyCode.R) && state == 0) {
            transform.position = initialPos;
        }
    }

    private void EnterElevator() {
        if (state == 0) { state = 1; sR.enabled = false; }
        else if(state == 1) { state = 0; sR.enabled = true; }
        GM.setState(state);
        Debug.Log("Current state " + state);
    }

}
