using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour {

    private TextMeshProUGUI scrapText;

    private GameObject notificationPanel;
    private TextMeshProUGUI notificationText;

    private float notificationT;
    // ----------------------------------------------------------------------------------------------------

    //
    private void Awake() {
        scrapText = GameObject.Find("Scrap Text").GetComponent<TextMeshProUGUI>();

        notificationPanel = GameObject.Find("Notification Panel");
        notificationText = GameObject.Find("Notification Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        SetNotificationText("", 0);
    }

    private void Update() {
        NotificationDisabler();
    }
    // ----------------------------------------------------------------------------------------------------

    //
    public void UpdateScrap(string text) {
        scrapText.text = text;
    }

    public void SetNotificationText(string text, float duration) {
        notificationPanel.SetActive(true);
        notificationText.text = text;
        notificationT = Time.time + duration;
    }

    private void NotificationDisabler() {
        if(Time.time >= notificationT) {
            notificationPanel.SetActive(false);
        }
    }
    // ----------------------------------------------------------------------------------------------------
}
