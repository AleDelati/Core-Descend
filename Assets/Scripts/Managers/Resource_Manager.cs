using UnityEngine;

public class Resource_Manager : MonoBehaviour {

    [SerializeField] int scrapMetal = 0;

    public void AddScrapMetal(int amount) {
        scrapMetal += amount;
    }

    public int ScrapMetal() {
        return scrapMetal;
    }

}
