using UnityEngine;

public class GameManager : MonoBehaviour {

    // Editor Config
    [SerializeField] int targetFPS = 60;
    [SerializeField] int vsync = 1;
    [SerializeField] int antiAliasing = 0;

    // Var
    Vector3 mouseWorldPos, mouseViewportPos;
    private int state = 0;  // 0 - Normal | 1 - Elevador | 2 - Interior del Elevador

    //
    void Start() {
        QualitySettings.vSyncCount = vsync;
        QualitySettings.antiAliasing = antiAliasing;
        Application.targetFrameRate = targetFPS;
    }

    void Update() {
        UpdateMousePos();
    }

    // Mouse
    private void UpdateMousePos() {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition); mouseWorldPos.z = 0;
        mouseViewportPos = Camera.main.ScreenToViewportPoint(UnityEngine.Input.mousePosition); mouseViewportPos.z = 0;
        //Debug.Log("Current mouse world position " + mouseWorldPos);
        //Debug.Log("Current mouse screen position" + mouseScreenPos);
    }

    public Vector3 GetMouseWorldPos() {
        return mouseWorldPos;
    }

    public Vector3 GetMouseViewportPos() {
        return mouseViewportPos;
    }

    //
    public void SetState(int _state) {
        state = _state;
    }

    public int GetState() {
        return state;
    }

}
