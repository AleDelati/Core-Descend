using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour {

    [SerializeField] GameObject player;
    [SerializeField] GameObject elevator;

    [SerializeField] float zoom;

    //private
    GameManager GM;
    GameObject target;

    private int state;

    private void Start() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();

        zoom = transform.GetComponent<Camera>().orthographicSize;
        target = player;
    }

    private void Update() {
        state = GM.getState();

        UpdateCamera();
    }

    private void UpdateCamera() {

        if(state == 0) { target = player; } else if ( state == 1) { target = elevator; }

        transform.SetPositionAndRotation(new Vector3(target.transform.position.x, target.transform.position.y, -10), Quaternion.identity);
        transform.GetComponent<Camera>().orthographicSize = zoom;
    }

}
