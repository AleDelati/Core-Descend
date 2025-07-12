using UnityEngine;

public class Block_Scrap : MonoBehaviour {

    [SerializeField] float hitPoints = 100;

    //
    private float currentHitpoints;

    private void Start() {
        currentHitpoints = hitPoints;
    }

    public void MineBlock(float damage) {
        currentHitpoints -= damage * Time.deltaTime;
        Debug.Log("Mining block, remaining hitpoints: " + currentHitpoints);
        if( currentHitpoints <= 0) { Destroy(gameObject); }
    }

}
