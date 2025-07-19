using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

    private TextMeshProUGUI scrapText;

    private void Start() {
        scrapText = GameObject.Find("Scrap Text").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScrap(string text) {
        scrapText.text = text;
    }

}
