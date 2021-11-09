using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEffect : MonoBehaviour
{
    [Header("listening Channel")]
    [SerializeField] VoidEventChannelSO _useRopeEvent;
    [SerializeField] VoidEventChannelSO _doneUseRopeEvent;
    public bool useSpeedEffect;

    private void Awake() {
        if(useSpeedEffect){
            _useRopeEvent.OnEventRaised += OnUseRope;
            _doneUseRopeEvent.OnEventRaised += OnDoneUseRope;

        }
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        _useRopeEvent.OnEventRaised -= OnUseRope;
        _doneUseRopeEvent.OnEventRaised -= OnDoneUseRope;
    }

    private void OnDoneUseRope()
    {
        gameObject.SetActive(false);
    }

    private void OnUseRope()
    {
        gameObject.SetActive(true);
    }
}
