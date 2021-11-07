using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupLocalPlayerFollowingCamera : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera _playerFollowCamera;

    [Header("Listening Channels")]
    [SerializeField] GameObjectEventChannelSO LocalPlayerReadyEvent;

    private void Awake() {
        _playerFollowCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    private void OnEnable() {
        LocalPlayerReadyEvent.OnEventRaised += OnLocalPlayerReady;
    }

    private void OnDisable() {
        LocalPlayerReadyEvent.OnEventRaised -= OnLocalPlayerReady;
    }

    private void OnLocalPlayerReady(GameObject localPlayer)
    {
        _playerFollowCamera.Follow = localPlayer.GetComponent<CharacterBehaviour>().CameraFollowTarget.transform;
    }
}
