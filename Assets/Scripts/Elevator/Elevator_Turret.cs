using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Elevator_Turret : MonoBehaviour {

    // Editor Config
    [Header("Turret IA")]
    public GameObject target = null;
    [SerializeField] float aimRaycastLenght = 50.0f;
    [SerializeField] Vector3 baseDetectionRange;
    [SerializeField] Vector3 detectionOffset;
    [Tooltip("El rango que tendra la torreta mientras este alertada")]
    [SerializeField] Vector3 alertedRange;
    [Tooltip("El tiempo que dura el estado de alerta de la torreta (En segundos)")]
    [SerializeField] float alertTime = 10;

    [Header("Turret  Config")]
    [Tooltip("Asigna el panel de control correspondiente a la torreta")]
    [SerializeField] GameObject turretPanel;
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

    // Child Ref
    private GameObject turretCanon;
    private GameObject turretLight;
    private GameObject projectileSpawnPoint;

    // Var
    private Vector3 mouseWorldPos;
    private Vector3 detectionRange;
    private bool inverted, disabled;
    private float alertT;
    private LayerMask raycastIgnore;
    // ----------------------------------------------------------------------------------------------------

    //
    private void OnDrawGizmosSelected() {
        // Area de deteccion
        if (detectionRange == Vector3.zero) {
            Gizmos.color = Color.green;
            if (!inverted) { Gizmos.DrawWireCube(transform.position + detectionOffset, baseDetectionRange); }
            else { Gizmos.DrawWireCube(transform.position + -detectionOffset, baseDetectionRange); }
        } else {
            if (!inverted) { Gizmos.DrawWireCube(transform.position + detectionOffset, detectionRange); }
            else { Gizmos.DrawWireCube(transform.position + -detectionOffset, detectionRange); }
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
        if (turretPanel.GetComponent<Turret_Panel>().GetRepairedStatus() == false) { DisableTurret(); }
    }

    private void Update() {
        if (!disabled) {
            CheckHP();
            CheckTargets();
            UpdateCanon();
        }
        if (detectionRange != baseDetectionRange) { AlertCD(); }
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
        mouseWorldPos = GM.GetMouseWorldPos(); mouseWorldPos.z = transform.position.z;
        switch (GM.GetPlayerState()) {
            case 0:     // Funcionamiento de las torretas si el jugador esta fuera del elevador
                TurretAutomatic();
                break;
            case 1:     // Funcionamiento de las torretas mientras el jugador esta conduciendo el elevador
                if (!inverted && mouseWorldPos.x > 0 || inverted && mouseWorldPos.x < 0) {  TurretManual(); } // El jugador controla manualmente las torretas del lado del elevador en el que se encuentra el mouse
                else { TurretAutomatic(); } // ---------------------------------------------------------------- Las torretas del lado contrario son controladas manualmente
                break;
            case 2:     // Funcionamiento de las torretas mientras el jugador esta en el interior del elevador
                TurretAutomatic();
                break;
            default:

                break;
        }
        
        
    }

    private void TurretAutomatic() {
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
            Shoot(false);
        }
    }

    private void TurretManual() {
        Vector3 localMousePos = transform.InverseTransformPoint(mouseWorldPos);
        float targetAngleLocal = Mathf.Atan2(localMousePos.y, localMousePos.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(targetAngleLocal, rotationLimit.y, rotationLimit.x);
        Quaternion targetRotation = Quaternion.Euler(0, 0, clampedAngle);
        turretCanon.transform.localRotation = Quaternion.Slerp(turretCanon.transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    // ----------------------------------------------------------------------------------------------------

    //
    private void CheckTargets() {

        // Busca torretas enemigas que sigan activas para asignar como objetivo
        if (target == null || target != null && !target.CompareTag("Enemy Turret")) {
            if (!inverted) {
                foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position + detectionOffset, detectionRange, 0)) {
                    if (collider.CompareTag("Enemy Turret") && collider.GetComponent<Health>().GetHP() != 0) {
                        target = collider.gameObject;
                        return;
                    }
                }
                target = null;
            } else {
                foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position - detectionOffset, detectionRange, 0)) {
                    if (collider.CompareTag("Enemy Turret") && collider.GetComponent<Health>().GetHP() != 0) {
                        target = collider.gameObject;
                        return;
                    }
                }
                target = null;
            }
        }

        // Verifica si el target fue destruido
        if (target != null) {
            if (target.GetComponent<Health>().GetHP() == 0) { target = null; }
        }

        // Verifica si las torretas salieron del rango
        if (target != null && target.CompareTag("Enemy Turret")) {
            if (!inverted) {
                foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position + detectionOffset, detectionRange, 0)) {
                    if (collider.CompareTag("Enemy Turret")) {
                        return;
                    }
                }
                target = null;
            } else {
                foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position - detectionOffset, detectionRange, 0)) {
                    if (collider.CompareTag("Enemy Turret")) {
                        return;
                    }
                }
                target = null;
            }
            
        }
        
        // Si los objetivos salen del rango anula el target
        if (target != null) {
            if (!inverted) {
                foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position + detectionOffset, detectionRange, 0)) {
                    if (collider.CompareTag("Enemy Turret")) {
                        return;
                    }
                }
                target = null;
            } else {
                foreach (Collider2D collider in Physics2D.OverlapBoxAll(transform.position - detectionOffset, detectionRange, 0)) {
                    if (collider.CompareTag("Enemy Turret")) {
                        return;
                    }
                }
                target = null;
            }
        }
    }

    //
    public void Shoot(bool manual) {
        if (!disabled) {
            if (manual) {   // Sistema de diaparo manual
                if (!inverted && mouseWorldPos.x > 0 || inverted && mouseWorldPos.x < 0) {
                    InstantiateProjectile();
                }
            } else {    // Sistema de disparo utilizado por la IA
                        // Crea un raycast en la direccion que esta apuntando la torreta
                RaycastHit2D hit;
                if (!inverted) {
                    hit = Physics2D.Raycast(projectileSpawnPoint.transform.position, turretCanon.transform.right * aimRaycastLenght, aimRaycastLenght, raycastIgnore);
                    Debug.DrawRay(projectileSpawnPoint.transform.position, turretCanon.transform.right * aimRaycastLenght, hit.collider != null ? Color.red : Color.green);
                } else {
                    hit = Physics2D.Raycast(projectileSpawnPoint.transform.position, -turretCanon.transform.right * aimRaycastLenght, aimRaycastLenght, raycastIgnore);
                    Debug.DrawRay(projectileSpawnPoint.transform.position, -turretCanon.transform.right * aimRaycastLenght, hit.collider != null ? Color.red : Color.green);
                }

                // Verifica que el raycast este impactando con el enemigo antes de disparar}
                // -!- El raycast causa un retraso entre cada disparo ya que la bala evita que pueda ver el target, funciona para nerfear la IA - Temporal
                if (hit.collider != null && hit.collider.CompareTag("Enemy Turret")) {
                    InstantiateProjectile();
                }
            }
        }
    }

    private void InstantiateProjectile() {
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

    // ----------------------------------------------------------------------------------------------------

    // Activa el modo de alerta de la torreta
    private void AlertTurret() {
        detectionRange = alertedRange;
        alertT = Time.time + alertTime;
        //Debug.Log("La torreta " + gameObject.name + " esta en estado de alerta.");
    }

    // Gestiona el tiempo que la torreta permanece alerta
    private void AlertCD() {
        if (Time.time > alertT) {
            detectionRange = baseDetectionRange;
            //Debug.Log("La torreta " + gameObject.name + " ya no esta en estado de alerta.");
        }
    }
    // ----------------------------------------------------------------------------------------------------

    // Gestion de HP de la torreta | Activacion/Desactivacion
    private void CheckHP() {
        if (health.GetHP() == 0) { DisableTurret(); }
    }

    private void DisableTurret() {
        Debug.Log("Torreta del elevador " + gameObject.name + " Desactivada");
        if (!inverted) { turretCanon.transform.rotation = new Quaternion(0, 0, -0.642787576f, 0.766044497f); } else { turretCanon.transform.rotation = new Quaternion(0, 0, 0.642787576f, 0.766044497f); }
        turretPanel.GetComponent<Turret_Panel>().SetRepaired(false);
        turretLight.GetComponent<Light2D>().enabled = false;
        disabled = true;
    }

    public void EnableTurret() {
        Debug.Log("Torreta del elevador " + gameObject.name + " Reactivada");
        turretLight.GetComponent<Light2D>().enabled = true;
        health.RestoreMaxHP();
        disabled = false;
    }
    // ----------------------------------------------------------------------------------------------------
}
