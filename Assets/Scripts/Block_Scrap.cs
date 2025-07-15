using UnityEngine;

public class Block_Scrap : MonoBehaviour {

    [SerializeField] float hitPoints = 100;
    [SerializeField] int scrapAmount = 10;

    //
    private float currentHitpoints;

    Resource_Manager rM;

    private void Start() {
        rM = GameObject.Find("Game Manager").GetComponent<Resource_Manager>();

        currentHitpoints = hitPoints;
    }

    public void MineBlock(float damage) {
        currentHitpoints -= damage * Time.deltaTime;
        Debug.Log("Mining block, remaining hitpoints: " + currentHitpoints);
        if( currentHitpoints <= 0) {
            Destroy(gameObject);
            rM.AddScrapMetal(scrapAmount);
        }
    }

}
