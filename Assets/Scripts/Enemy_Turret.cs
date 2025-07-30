using UnityEngine;

public class Enemy_Turret : MonoBehaviour {

    // Editor Config
    [Header("Turret Config")]
    [SerializeField] GameObject target = null;
    [SerializeField] float detectionRange = 20;

    [Tooltip("Limite de Rotacion del Cañon X = Superior Y = Inferior")]
    [SerializeField] Vector2 rotationLimit;
    [Range(1f, 20f)]

    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float gizmoRayLenght = 2.0f;

    [Header("Projectile Config")]
    [SerializeField] GameObject projectile_Prefab;
    [SerializeField] float shotCD = 0.5f;
    private GameObject lastProjectile;
    private float nextShotTime;

    // Main Ref
    GameManager GM;

    // Ext Ref

    // Child Ref
    private GameObject turretCanon;
    private GameObject turretLight;
    private GameObject projectileSpawnPoint;

    // Var
    public bool inverted;

    //
    private void OnDrawGizmosSelected() {
        if(turretCanon!= null) {
            if (!inverted) {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(turretCanon.transform.position, turretCanon.transform.right * gizmoRayLenght);
            } else {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(turretCanon.transform.position, -turretCanon.transform.right * gizmoRayLenght);
            }
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void Awake() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        turretCanon = transform.Find("Turret Canon").gameObject;
        turretLight = turretCanon.transform.Find("Turret Light").gameObject;
        projectileSpawnPoint = turretCanon.transform.Find("Projectile Spawn Point").gameObject;
    }

    private void Start() {
        inverted = transform.lossyScale.x < 0;
        if (!inverted) { turretLight.transform.rotation = new Quaternion(0, 0, -0.707106829f, 0.707106829f); } else { turretLight.transform.rotation = new Quaternion(0, 0, 0.707106829f, 0.707106829f); }
    }

    private void Update() {
        CheckTargets();
        UpdateCanon();
    }

    private void UpdateCanon() {
        if (target != null) {
            Vector3 targetPos = target.transform.position; targetPos.z = transform.position.z;
            Vector3 localMousePos = transform.InverseTransformPoint(targetPos);
            float targetAngleLocal = Mathf.Atan2(localMousePos.y, localMousePos.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(targetAngleLocal, rotationLimit.y, rotationLimit.x);
            Quaternion targetRotation = Quaternion.Euler(0, 0, clampedAngle);
            turretCanon.transform.localRotation = Quaternion.Slerp(turretCanon.transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        } else {    // Torreta sin objetivo
            Debug.Log("La torreta no tiene objetivos");
        } 
    }

    private void CheckTargets() {   //Añadir raycasts

        // Prioriza las torretas del elevador
        if (target == null || target != null && !target.CompareTag("Elevator Turret")) {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, detectionRange)) {
                if (collider.CompareTag("Elevator Turret")) {
                    target = collider.gameObject;
                    return;
                }
            }
            target = null;
        }

        // Verifica si las torretas salieron del rango
        if (target != null && target.CompareTag("Elevator Turret")) {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, detectionRange)) {
                if (collider.CompareTag("Elevator Turret")) {
                    return;
                }
            }
            target = null;
        }

        // Si no hay torretas dentro del rango ataca el casco del elevador
        if (target == null) {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, detectionRange)) {
                if (collider.CompareTag("Elevator")) {
                    target = collider.gameObject;
                    return;
                }
            }
        }

        // Si los objetivos salen del rango anula el target
        if (target != null) {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, detectionRange)) {
                if (collider.CompareTag("Elevator") || collider.CompareTag("Elevator Turret")) {
                    return;
                }
            }
            target = null;
        }
    }

    /*
    public void Shoot() {
        if (!inverted && mouseWorldPos.x > 0 || inverted && mouseWorldPos.x < 0) {
            if (Time.time >= nextShotTime) {
                nextShotTime = Time.time + shotCD;
                lastProjectile = Instantiate(projectile_Prefab, projectileSpawnPoint.transform.position, turretCanon.transform.rotation);
                Projectile proyectile = lastProjectile.GetComponent<Projectile>();
                if (proyectile != null) {
                    Vector2 targetDir;
                    if (!inverted) { targetDir = turretCanon.transform.right; } else { targetDir = -turretCanon.transform.right; }
                    proyectile.SetDirection(targetDir);
                }
            }
        }
    }
    */
}
