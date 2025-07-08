using UnityEngine;

public class CameraManager : MonoBehaviour {

    [SerializeField] GameObject player;
    [SerializeField] GameObject elevator;

    [SerializeField] float characterZoom;

    [SerializeField] float elevatorZoom;
    [SerializeField] float elevatorOffset = 5;

    //private
    GameManager GM;
    GameObject target;

    private int state;

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        target = player;
    }

    private void Update() {
        state = GM.GetState();

        UpdateCamera();
    }

    private void UpdateCamera() {

        Vector2  currentOffset = Vector2.zero;

        if(state == 0) {
            target = player;
            transform.GetComponent<Camera>().orthographicSize = characterZoom;
        }
        else if ( state == 1) {
            target = elevator;
            transform.GetComponent<Camera>().orthographicSize = elevatorZoom;

            // Offset de la camara dependiendo de la direccion de movimiento del elevador
            if (elevator.GetComponent<Elevator_Controller>().GetLastInput() == 0){  }
            else if((elevator.GetComponent<Elevator_Controller>().GetLastInput() == 1)){ currentOffset = new Vector2(0, elevatorOffset); }
            else if((elevator.GetComponent<Elevator_Controller>().GetLastInput() == -1)){ currentOffset = new Vector2(0, -elevatorOffset); }
        }

        transform.SetPositionAndRotation(new Vector3(target.transform.position.x + currentOffset.x, target.transform.position.y + currentOffset.y, -10), Quaternion.identity);
    }

}
