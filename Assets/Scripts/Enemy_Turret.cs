using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy_Turret : MonoBehaviour {

    // Editor Config
    [Header("Turret IA")]
    public GameObject target = null;
    [SerializeField] float aimRaycastLenght = 50.0f;
    [SerializeField] float baseDetectionRange = 20;

    [Tooltip("El rango que tendra la torreta mientras este alertada")]
    [SerializeField] float alertedRange = 50;
    [Tooltip("El tiempo que dura el estado de alerta de la torreta (En segundos)")]
    [SerializeField] float alertTime = 10;

    [Header("Turret Config")]
    [Tooltip("Limite de Rotacion del Cañon X = Superior Y = Inferior")]
    [SerializeField] Vector2 rotationLimit;
    
    [Range(1f, 20f)]
    [SerializeField] float rotationSpeed = 10f;

    [Header("Projectile Config")]
    [SerializeField] GameObject projectile_Prefab;
    [SerializeField] float shotCD = 0.5f;
    [Tooltip("Un valor (Angulo) utilizado para añadir dispersion a los proyectiles disparados.")]
    [Range(0f, 15f)]
    [SerializeField] float projectileSpreadAngle = 3f;
    private GameObject lastProjectile;
    private float nextShotTime;

    // Main Ref
    GameManager GM;
    Event_Manager Evt_M;

    // Self Ref
    Health health;

    // Ext Ref

    // Child Ref
    private GameObject turretCanon;
    private GameObject turretLight;
    private GameObject projectileSpawnPoint;

    // Var
    public bool inverted;
    private bool disabled = false;
    public float detectionRange, alertT;
    private LayerMask raycastIgnore;

    string l_NoTargetColor = "#FFFFFFFF", l_TargetColor = "#C83232FF";

    //
    private void OnDrawGizmosSelected() {
        // Area de deteccion
        if(detectionRange == 0) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, baseDetectionRange);
        } else {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
        
    }

    private void Awake() {
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        Evt_M = GameObject.Find("Game Manager").GetComponent<Event_Manager>();
        health = GetComponent<Health>();
        turretCanon = transform.Find("Turret Canon").gameObject;
        turretLight = turretCanon.transform.Find("Turret Light").gameObject;
        projectileSpawnPoint = turretCanon.transform.Find("Projectile Spawn Point").gameObject;

        raycastIgnore = ~0; raycastIgnore &= ~(LayerMask.NameToLayer("Ignore Raycast"));
    }

    private void Start() {
        inverted = transform.lossyScale.x < 0;
        if (!inverted) { turretLight.transform.rotation = new Quaternion(0, 0, -0.707106829f, 0.707106829f); } else { turretLight.transform.rotation = new Quaternion(0, 0, 0.707106829f, 0.707106829f); }
    }

    private void Update() {
        if (!disabled) {
            CheckHP();
            CheckTargets();
            UpdateCanon();
            if(detectionRange != baseDetectionRange){ AlertCD(); }
        }
    }

    private void OnEnable() {
        Event_Manager.onElevatorTriggerAlarm += AlertTurret;
    }

    private void OnDisable() {
        Event_Manager.onElevatorTriggerAlarm -= AlertTurret;
    }
    // ----------------------------------------------------------------------------------------------------

    //
    private void UpdateCanon() {
        if (target != null) {
            //Asigna la posicion del target dependiendo de si esta mas arriba o mas abajo en Y, para mejorar la precision
            Vector3 targetPos; float targetDistance = target.transform.position.y - transform.position.y;
            //Debug.Log("Target Distance " + targetDistance);
            if (targetDistance < -5) { targetPos = target.GetComponent<Collider2D>().bounds.max; targetPos.z = transform.position.z; }
            else if (targetDistance >= -5 && targetDistance <= 5) { targetPos = target.GetComponent<Collider2D>().bounds.center; targetPos.z = transform.position.z; }
            else { targetPos = target.GetComponent<Collider2D>().bounds.min; targetPos.z = transform.position.z; }

            Vector3 localMousePos = transform.InverseTransformPoint(targetPos);
            float targetAngleLocal = Mathf.Atan2(localMousePos.y, localMousePos.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(targetAngleLocal, rotationLimit.y, rotationLimit.x);
            Quaternion targetRotation = Quaternion.Euler(0, 0, clampedAngle);
            turretCanon.transform.localRotation = Quaternion.Slerp(turretCanon.transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            Shoot();
        } else {    // Torreta sin objetivo
            //Debug.Log("La torreta no tiene objetivos");
        } 
    }

    private void CheckTargets() {

        // Cambia el color de las luces de la torreta dependiendo de si tiene un objetivo o no
        Color parsedColor;
        if (target == null) {
            if (UnityEngine.ColorUtility.TryParseHtmlString(l_NoTargetColor, out parsedColor)) {
                turretLight.GetComponent<Light2D>().color = parsedColor;
            }
        } else {
            if (UnityEngine.ColorUtility.TryParseHtmlString(l_TargetColor, out parsedColor)) {
                turretLight.GetComponent<Light2D>().color = parsedColor;
            }
        }

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

    private void Shoot() {
        // Crea un raycast en la direccion que esta apuntando la torreta
        RaycastHit2D hit;
        if (!inverted) {
            hit = Physics2D.Raycast(projectileSpawnPoint.transform.position, turretCanon.transform.right * aimRaycastLenght, aimRaycastLenght, raycastIgnore);
            Debug.DrawRay(projectileSpawnPoint.transform.position, turretCanon.transform.right * aimRaycastLenght, hit.collider != null ? Color.red : Color.green);
        } else {
            hit = Physics2D.Raycast(projectileSpawnPoint.transform.position, -turretCanon.transform.right * aimRaycastLenght, aimRaycastLenght, raycastIgnore);
            Debug.DrawRay(projectileSpawnPoint.transform.position, -turretCanon.transform.right * aimRaycastLenght, hit.collider != null ? Color.red : Color.green);
        }

        // Verifica que el raycast este impactando con el enemigo antes de disparar
        if (hit.collider != null) {
            if (hit.collider.CompareTag("Elevator") || hit.collider.CompareTag("Elevator Turret")) {
                if (Time.time >= nextShotTime) {
                    nextShotTime = Time.time + shotCD;
                    lastProjectile = Instantiate(projectile_Prefab, projectileSpawnPoint.transform.position, turretCanon.transform.rotation);
                    Projectile proyectile = lastProjectile.GetComponent<Projectile>();
                    if (proyectile != null) {
                        Vector2 targetDir;
                        if (!inverted) { targetDir = turretCanon.transform.right; } else { targetDir = -turretCanon.transform.right; }

                        // Crea un angulo aleatorio para añadir el efecto de dispersion a la trayectoria final de la bala
                        float randomAngle = Random.Range(-projectileSpreadAngle, projectileSpreadAngle);
                        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
                        targetDir = spreadRotation * targetDir;

                        proyectile.SetDirection(targetDir);
                    }
                    Evt_M.OnElevatorTriggerAlarm();
                }
            }
        }
    }
    // ----------------------------------------------------------------------------------------------------

    // Activa el modo de alerta de la torreta
    private void AlertTurret() {
        detectionRange = alertedRange;
        alertT = Time.time + alertTime;
        //Debug.Log("La torreta " + gameObject.name + " esta en estado de alerta.");
    }

    // Gestiona el tiempo que la torreta permanece alerta
    private void AlertCD() {
        if(Time.time > alertT) { 
            detectionRange = baseDetectionRange;
            //Debug.Log("La torreta " + gameObject.name + " ya no esta en estado de alerta.");
        }
    }
    // ----------------------------------------------------------------------------------------------------

    //
    private void CheckHP() {
        if (health.GetHP() == 0) { DisableTurret(); }
    }

    private void DisableTurret() {
        if (!inverted) { turretCanon.transform.rotation = new Quaternion(0, 0, -0.642787576f, 0.766044497f); }
        else { turretCanon.transform.rotation = new Quaternion(0, 0, 0.642787576f, 0.766044497f); }
        turretLight.GetComponent<Light2D>().enabled = false;
        disabled = true;
    }
    // ----------------------------------------------------------------------------------------------------

    public bool GetState() { return disabled; }
}
