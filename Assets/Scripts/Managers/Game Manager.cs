using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] int targetFPS = 60;
    [SerializeField] int vsync = 1;

    //private

    private int state = 0;  // 0 - Normal | 1 - Elevador

    void Start() {
        QualitySettings.vSyncCount = vsync;
        Application.targetFrameRate = targetFPS;
    }

    void Update() {
        
    }

    public void SetState(int _state) {
        state = _state;
    }

    public int GetState() {
        return state;
    }


}
