using UnityEngine;

public enum projectileT { Player, Enemy }

public class Projectile : MonoBehaviour {

    // Editor Config
    [Header("Projectile Config")]
    [SerializeField] float speed = 10.0f;
    [SerializeField] float damage = 1.0f;
    [SerializeField] float lifeTime = 3.0f;
    [SerializeField] projectileT projectileType;

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

        // Si los proyectiles impactan el elevador, a los decals de impacto se les asigna el elevador como parent para que sigan su movimiento
        if(collision.CompareTag("Elevator") || collision.CompareTag("Elevator Turret")) { lastInstance.transform.parent = collision.gameObject.transform; }
        
        // Verifica la logica de impactos del proyectil dependiendo de si fue disparado por le jugador o los enemigos
        switch (projectileType) {
            case projectileT.Player:
                if (collision.CompareTag("Enemy Turret")) {
                    collision.GetComponent<Health>().GetHit(damage);
                }
                break;
            case projectileT.Enemy:
                if (collision.CompareTag("Elevator") || collision.CompareTag("Elevator Turret")) {
                    //collision.GetComponent<Health>().GetHit(damage);
                }
                break;
            default:
                Debug.LogWarning("Tipo de proyectil desconocido");
                break;
        }

        Destroy(gameObject);
    }

    public void SetDirection(Vector2 dir) {
        movementDir = dir.normalized;
        float angle = Mathf.Atan2(movementDir.y, movementDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
