using UnityEngine;

public class Health : MonoBehaviour {

    // Editor Config
    [Header("Health Config")]
    [SerializeField] float maxHP = 10;
    [SerializeField] float initialHP = 5;

    public float HP;

    private void Start() {
        HP = initialHP;
    }

    public float GetHP() {
        return HP;
    }

    public void GetHit(float damage) {
        HP -= damage;
        if (HP < 0) {  HP = 0; }
    }

}
