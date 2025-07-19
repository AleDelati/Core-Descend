using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] int targetFPS = 60;
    [SerializeField] int vsync = 1;
    [SerializeField] int antiAliasing = 0;

    //private

    private int state = 0;  // 0 - Normal | 1 - Elevador | 1 - Interior del Elevador

    void Start() {
        QualitySettings.vSyncCount = vsync;
        QualitySettings.antiAliasing = antiAliasing;
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
