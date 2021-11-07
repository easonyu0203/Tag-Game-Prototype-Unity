using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game{

    public class InputHandler : MonoBehaviour
    {
        [Header("Listening Channels")]
        public GameObjectEventChannelSO LocalCharacterReadyEvent;

        private bool _isLocalCharacterReady = false;

        private MoveRotateController _moveRotateController;
        private PlayerAssetInput _input;

        private void Awake() {
            _input = GetComponent<PlayerAssetInput>();
            _moveRotateController = GetComponent<MoveRotateController>();

        }

        private void OnEnable() {
            LocalCharacterReadyEvent.OnEventRaised += OnLocalCHaracterReady;
        }

        private void OnDisable() {
            LocalCharacterReadyEvent.OnEventRaised -= OnLocalCHaracterReady;
        }

        private void OnLocalCHaracterReady(GameObject localCharacter)
        {
            // set ready to true
            _isLocalCharacterReady = true;
        }

        private void OnJump()
        {
            if(_isLocalCharacterReady == false) return;
            
            _moveRotateController.Jump();
        }

        private void LateUpdate() {
            if(_isLocalCharacterReady == false) return;
            
            _moveRotateController.CameraRotation(_input.look);
        }

        private void FixedUpdate() {
            if(_isLocalCharacterReady == false) return;

            _moveRotateController.WalkingMove(_input.move);
        }
    }

}
