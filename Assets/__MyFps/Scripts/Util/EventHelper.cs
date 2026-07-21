using UnityEngine;
using UnityEngine.Events;

public class EventHelper : MonoBehaviour
{

    public UnityEvent onEvent;

    public void InvokeEvent() => onEvent.Invoke();

    public UnityEvent onEnableEvent;
    private void OnEnable() => onEnableEvent?.Invoke();
}