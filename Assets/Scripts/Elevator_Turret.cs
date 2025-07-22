using UnityEngine;

public class Elevator_Turret : MonoBehaviour {

    // Editor Config
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
    Elevator_Controller elevatorC;

    // Child Ref
    private GameObject turretCanon;
    private GameObject turretLight;
    private GameObject projectileSpawnPoint;

    // Var
    private Vector3 mouseWorldPos;
    public bool inverted;

    //
    private void OnDrawGizmosSelected() {
        if (!inverted) {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(turretCanon.transform.position, turretCanon.transform.right * gizmoRayLenght);
        } else {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(turretCanon.transform.position, -turretCanon.transform.right * gizmoRayLenght);
        }
        
    }

    private void Awake() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        elevatorC = GameObject.Find("Elevator").GetComponent<Elevator_Controller>();
        turretCanon = transform.Find("Turret Canon").gameObject;
        turretLight = turretCanon.transform.Find("Turret Light").gameObject;
        projectileSpawnPoint = turretCanon.transform.Find("Projectile Spawn Point").gameObject;
    }

    private void Start() {
        inverted = transform.lossyScale.x < 0;
        if (!inverted) { turretLight.transform.rotation = new Quaternion(0, 0, -0.707106829f, 0.707106829f); } else { turretLight.transform.rotation = new Quaternion(0, 0, 0.707106829f, 0.707106829f); }
    }

    private void Update() {
        if(GM.GetState() == 1 && elevatorC.GetTurretStatus()){ UpdateCanon(); }
    }

    private void UpdateCanon() {
        mouseWorldPos = GM.GetMouseWorldPos(); mouseWorldPos.z = transform.position.z;
        if (!inverted && mouseWorldPos.x > 0 || inverted && mouseWorldPos.x < 0) {
            Vector3 localMousePos = transform.InverseTransformPoint(mouseWorldPos);
            float targetAngleLocal = Mathf.Atan2(localMousePos.y, localMousePos.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(targetAngleLocal, rotationLimit.y, rotationLimit.x);
            Quaternion targetRotation = Quaternion.Euler(0, 0, clampedAngle);
            turretCanon.transform.localRotation = Quaternion.Slerp(turretCanon.transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            //turretCanon.transform.localRotation = Quaternion.Euler(0, 0, clampedAngle);
        }
    }

    public void Shoot() {
        if (!inverted && mouseWorldPos.x > 0 || inverted && mouseWorldPos.x < 0) {
            if(Time.time >= nextShotTime) {
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

}
