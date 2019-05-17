using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoEnableEvent : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnEnableEvent;

    [SerializeField]
    UnityEvent OnDisableEvent;

    private void OnEnable()
    {
        OnEnableEvent.Invoke();
    }

    private void OnDisable()
    {
        OnDisableEvent.Invoke();
    }
}
