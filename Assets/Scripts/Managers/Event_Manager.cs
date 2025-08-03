using System;
using UnityEngine;

public class Event_Manager : MonoBehaviour {

    // Events
    public static event Action onElevatorTriggerAlarm;

    public void OnElevatorTriggerAlarm() {  // Se activa cuando cualquier torreta dispara, activando el modo alerta
        onElevatorTriggerAlarm?.Invoke();
    }

}
