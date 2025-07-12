using UnityEngine;

public class Character_Controller : MonoBehaviour {

    [Header("Movimiento")]
    [SerializeField] float speed = 8;

    [Header("Herramientas")]
    [SerializeField] float miningRange = 2;
    [SerializeField] float miningDamage = 10;

    // private
    
    GameManager GM;
    GameObject elevator;
    Elevator_Controller elevatorC;

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    SpriteRenderer sR;


    //

    Vector3 initialPos;
    Vector3 mousePos;
    int state = 0;  // 0 - Normal | 1 - Elevador
    int lastInput = 0;
    bool inElevator = false;
    

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        elevator = GameObject.Find("Elevator");
        elevatorC = elevator.GetComponent<Elevator_Controller>();

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sR = GetComponent<SpriteRenderer>();

        initialPos = transform.position;
    }

    private void Update() {
        Inputs();
    }

    private void Inputs() {

        mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition); mousePos.z = 0;
        //Debug.Log("Current mouse position " + mousePos);

        if (UnityEngine.Input.GetMouseButton(0)) {
            MiningDetection();
        }
        if (UnityEngine.Input.GetKey(KeyCode.A) && state == 0) {
            lastInput = 0;
            transform.position = transform.position + new Vector3(-1, 0) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKey(KeyCode.D) && state == 0) {
            lastInput = 1;
            transform.position = transform.position + new Vector3(1, 0) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKey(KeyCode.W) && state == 0) {
            transform.position = transform.position + new Vector3(0, 1) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKey(KeyCode.S) && state == 0) {
            transform.position = transform.position + new Vector3(0, -1) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.E) && elevatorC.GetProximity()) {
            DriveElevator();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && state == 0) {
            EnterElevator();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.R) && state == 0) {
            transform.position = initialPos;
        }

        UpdateSprite();
        // Mueve el robot junto con el elevador mientras se esta controlando al elevador
        if (state == 1){ transform.position = elevatorC.transform.position + new Vector3(0, 5, 0); }
    }



    //
    private void DriveElevator() {
        if (state == 0) { state = 1; sR.enabled = false; }
        else if(state == 1) { state = 0; sR.enabled = true; }
        GM.SetState(state);
        Debug.Log("Conduciendo elevador " + state);
    }

    private void EnterElevator() {
        if (!inElevator && elevatorC.GetProximity()) {
            Debug.Log("Entrando al elevador " + state);
            transform.position = new Vector3(0, 105, 0);
            inElevator = true;
        } else if (inElevator) {
            Debug.Log("Saliendo del elevador " + state);
            transform.position = elevator.transform.position + new Vector3(0, 5, 0);
            inElevator = false;
        }
        
    }

    // Verifica el click sobre un bloque destruible y si esta dentro del rango de mineria
    private void MiningDetection() {
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null && hit.collider.tag == "Scrap") {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, miningRange)) {
                if (collider.gameObject == hit.collider.gameObject) {
                    hit.collider.gameObject.GetComponent<Block_Scrap>().MineBlock(miningDamage);
                }
            }
            //false
        }
    }

    //
    private void UpdateSprite() {
        if (lastInput == 0){ sR.flipX = false; }
        if (lastInput == 1){ sR.flipX = true; }
    }

}
