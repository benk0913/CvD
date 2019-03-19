using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorAssist : MonoBehaviour
{
    [SerializeField]
    List<UnityEvent> AnimatorEvent;

    public void InvokeAnimatorEvent(int index)
    {
        AnimatorEvent[index].Invoke();
    }
}
