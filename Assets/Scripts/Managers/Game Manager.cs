using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] int targetFPS = 60;
    [SerializeField] int vsync = 1;

    void Start() {
        QualitySettings.vSyncCount = vsync;
        Application.targetFrameRate = targetFPS;
    }

    void Update() {
        
    }
}
