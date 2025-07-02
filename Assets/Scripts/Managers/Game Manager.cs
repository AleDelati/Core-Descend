using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] int targetFPS = 60;
    [SerializeField] int vsync = 1;

    //private

    private int state = 0;

    void Start() {
        QualitySettings.vSyncCount = vsync;
        Application.targetFrameRate = targetFPS;
    }

    void Update() {
        
    }

    public void setState(int _state) {
        state = _state;
    }

    public int getState() {
        return state;
    }


}
