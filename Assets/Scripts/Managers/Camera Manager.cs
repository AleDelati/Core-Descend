using UnityEngine;

public class CameraManager : MonoBehaviour {

    [Header ("Player Config")]
    [SerializeField] float characterZoom;

    [Header ("Elevator Config")]
    [SerializeField] float elevatorZoom;
    [SerializeField] float elevatorOffset = 5;

    //private
    GameObject player;
    GameObject elevator;

    GameManager GM;
    GameObject target;

    private int state;  // 0 - Normal | 1 - Elevador | 1 - Interior del Elevador

    private void Start() {
        player = GameObject.Find("Player");
        elevator = GameObject.Find("Elevator");

        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        target = player;
    }

    private void Update() {
        state = GM.GetState();

        UpdateCamera();
    }

    private void UpdateCamera() {

        Vector2  currentOffset = Vector2.zero;

        if(state == 0 || state == 2) {  // Seguimiento player
            target = player;
            transform.GetComponent<Camera>().orthographicSize = characterZoom;
        }
        else if ( state == 1) {         // Seguimiento elevador
            target = elevator;
            transform.GetComponent<Camera>().orthographicSize = elevatorZoom;

            // Offset de la camara dependiendo de la direccion de movimiento del elevador
            if (player.GetComponent<Character_Controller>().GetMouseScreenPos().y >= 0.90) { currentOffset = new Vector2(0, elevatorOffset); }
            else if(player.GetComponent<Character_Controller>().GetMouseScreenPos().y <= 0.30) { currentOffset = new Vector2(0, -elevatorOffset); }
        }

        transform.SetPositionAndRotation(new Vector3(target.transform.position.x + currentOffset.x, target.transform.position.y + currentOffset.y, -10), Quaternion.identity);
    }

}
