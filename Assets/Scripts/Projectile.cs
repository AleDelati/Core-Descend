using UnityEngine;

public class Projectile : MonoBehaviour {

    // Editor Config
    [Header("Projectile Config")]
    [SerializeField] float speed = 10.0f;
    [SerializeField] float damage = 1.0f;
    [SerializeField] float lifeTime = 3.0f;

    [SerializeField] GameObject impact_Prefab;
    private GameObject lastInstance;

    // Var
    private Vector2 movementDir;

    //
    private void Start() {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate() {
        transform.Translate(movementDir * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //Debug.Log("Proyectil impacto: " + collision.name);
        if(movementDir.x > 0){ 
            lastInstance = Instantiate(impact_Prefab, new Vector2(Mathf.Round(transform.position.x) - 0.35f, transform.position.y), Quaternion.identity);
        }
        if(movementDir.x < 0){ 
            lastInstance = Instantiate(impact_Prefab, new Vector2(Mathf.Round(transform.position.x) + 0.35f, transform.position.y), Quaternion.identity);
            lastInstance.GetComponent<Impact_Decal>().FlipSprite();
        }
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 dir) {
        movementDir = dir.normalized;
        float angle = Mathf.Atan2(movementDir.y, movementDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
