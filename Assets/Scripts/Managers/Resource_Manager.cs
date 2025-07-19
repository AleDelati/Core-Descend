using UnityEngine;

public class Resource_Manager : MonoBehaviour {

    [SerializeField] int scrapMetal = 0;

    private UI_Manager uiManager;

    private void Start() {
        uiManager = GetComponent<UI_Manager>();
    }

    //
    public void AddScrapMetal(int amount) {
        scrapMetal += amount;
        uiManager.UpdateScrap(scrapMetal.ToString());
    }

    public void SubtractScrapMetal(int amount) {
        scrapMetal -= amount;
        uiManager.UpdateScrap(scrapMetal.ToString());

    }

    public int ScrapMetal() {
        return scrapMetal;
    }

}
