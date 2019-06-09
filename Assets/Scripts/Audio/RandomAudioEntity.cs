﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioEntity : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> AudioClips = new List<AudioClip>();

    private void OnEnable()
    {

        AudioControl.Instance.PlayInPosition(AudioClips[Random.Range(0, AudioClips.Count)], transform.position);
    }
}
