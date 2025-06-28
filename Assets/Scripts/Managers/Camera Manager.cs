using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour {

    [SerializeField] GameObject target;

    [SerializeField] float zoom;

    private void Start() {
        zoom = transform.GetComponent<Camera>().orthographicSize;
    }

    private void Update() {
        transform.SetPositionAndRotation(new Vector3(target.transform.position.x, target.transform.position.y, -10), Quaternion.identity);
        transform.GetComponent<Camera>().orthographicSize = zoom;
    }

}
