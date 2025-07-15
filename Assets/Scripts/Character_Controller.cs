using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Character_Controller : MonoBehaviour {

    [Header("Movimiento")]
    [SerializeField] float speed = 8;

    [Header("Herramientas")]
    [SerializeField] float miningRange = 2;
    [SerializeField] float miningDamage = 10;

    // private

    // Main Ref
    GameManager GM;
    Resource_Manager rM;

    // GameObject Ref
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    SpriteRenderer sR;
    
    // External Ref
    GameObject elevator;
    Elevator_Controller elevatorC;

    // Child Ref
    GameObject robot_Light;

    // Var
    Vector3 initialPos;
    Vector3 mouseWorldPos; Vector3 mouseScreenPos;
    int state = 0;  // 0 - Normal | 1 - Elevador    | 2 - Interior
    int lastInput = 0;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        rM = GameObject.Find("Game Manager").GetComponent<Resource_Manager>();

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sR = GetComponent<SpriteRenderer>();

        elevator = GameObject.Find("Elevator");
        elevatorC = elevator.GetComponent<Elevator_Controller>();

        robot_Light = transform.Find("Light").gameObject;

        initialPos = transform.position;
    }

    private void Update() {
        Inputs();
    }

    private void Inputs() {

        UpdateMouseInputs();

        if (UnityEngine.Input.GetMouseButton(0)) {
            MiningDetection();
        }
        if (UnityEngine.Input.GetKey(KeyCode.A) && Check_EnableToMove()) {
            lastInput = 0;
            transform.position = transform.position + new Vector3(-1, 0) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKey(KeyCode.D) && Check_EnableToMove()) {
            lastInput = 1;
            transform.position = transform.position + new Vector3(1, 0) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKey(KeyCode.W) && Check_EnableToMove()) {
            transform.position = transform.position + new Vector3(0, 1) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKey(KeyCode.S) && Check_EnableToMove()) {
            transform.position = transform.position + new Vector3(0, -1) * speed * Time.deltaTime;
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.E) && elevatorC.GetProximity()) {
            DriveElevator();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) {
            if(state == 1 || state == 2){ EnterElevator(); }
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.R) && state == 0) {
            transform.position = initialPos;
        }

        //
        if (UnityEngine.Input.GetKeyDown(KeyCode.F1)) {
            Debug.Log("Scrap Metal: " + rM.ScrapMetal());
        }

        UpdateSprite();
        // Mueve el robot junto con el elevador mientras se esta controlando al elevador
        if (state == 1){ transform.position = elevatorC.transform.position + new Vector3(0, 5, 0); }
    }



    //
    private void DriveElevator() {
        if (state == 0) { 
            state = 1;
            Debug.Log("Conduciendo el Elevador");
            ToggleCharacter(false);
        }
        else if(state == 1) {
            state = 0;
            Debug.Log("Saliendo del Elevador");
            ToggleCharacter(true);
        }
        GM.SetState(state);
    }

    private void EnterElevator() {

        if (state == 1) {
            Debug.Log("Entrando al interior del Elevador");
            state = 2;

            ToggleCharacter(true);
            transform.position = new Vector3(0, 105, 0);
        }else if (state == 2) {
            Debug.Log("Volviendo a conducir el Elevador");
            state = 1;

            ToggleCharacter(false);
        }
        GM.SetState(state);
    }

    // Verifica el click sobre un bloque destruible y si esta dentro del rango de mineria
    private void MiningDetection() {
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
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
        if (lastInput == 0) { sR.flipX = false; robot_Light.transform.rotation = new Quaternion(0,0,0.707106829f,0.707106829f); }
        if (lastInput == 1) { sR.flipX = true; robot_Light.transform.rotation = new Quaternion(0, 0, -0.707106829f, 0.707106829f); }
    }

    //
    private bool Check_EnableToMove() {
        if(state == 0 || state == 2){ return true; }
        else { return false; }
    }

    //
    private void ToggleCharacter(bool active) {
        sR.enabled = active;
        robot_Light.GetComponent<Light2D>().enabled = active;
    }

    //  Mouse
    private void UpdateMouseInputs() {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition); mouseWorldPos.z = 0;
        mouseScreenPos = Camera.main.ScreenToViewportPoint(UnityEngine.Input.mousePosition); mouseScreenPos.z = 0;
        //Debug.Log("Current mouse world position " + mouseWorldPos);
        //Debug.Log("Current mouse screen position" + mouseScreenPos);
    }

    public Vector3 GetMouseWorldPos() {
        return mouseWorldPos;
    }

    public Vector3 GetMouseScreenPos() {
        return mouseScreenPos;
    }

}
