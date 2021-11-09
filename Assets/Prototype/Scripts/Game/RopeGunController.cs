using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public class RopeGunController : MonoBehaviour
    {

        [Header("Listening Channel")]
        [SerializeField] GameObjectEventChannelSO LocalCharacterReadyEvent;


        // reference
        private RaycastHit _lookRaycastHit = default;
        private GameObject _localCharacter;
        private GameObject _gunShootPoint;
        private NetworkRopeGun _networkRopeGun;

        private Camera _cameraComponent;

        private void OnEnable() {
			LocalCharacterReadyEvent.OnEventRaised += OnLocalCharacterReady;
		}

		private void OnDisable() {
			LocalCharacterReadyEvent.OnEventRaised -= OnLocalCharacterReady;
		}

        private void OnLocalCharacterReady(GameObject localCharacter)
        {
            _localCharacter = localCharacter;
            _gunShootPoint = localCharacter.GetComponent<CharacterBehaviour>().GunShootPoint;
            _networkRopeGun = localCharacter.GetComponent<NetworkRopeGun>();
        }

        // Start is called before the first frame update
        void Start()
        {
            _cameraComponent = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        }

        private void FixedUpdate() {
            //get aim point
            Ray ray = _cameraComponent.ScreenPointToRay(new Vector3(_cameraComponent.pixelWidth / 2, _cameraComponent.pixelHeight / 2, 0));
		    Physics.Raycast(ray, out _lookRaycastHit);
        }

        public void Shoot(){
            if(_localCharacter == null) return;

            // find direction
            Vector3 ropeDirection = (_lookRaycastHit.point - _gunShootPoint.transform.position).normalized;

            _networkRopeGun.ShootRope(ropeDirection);
        }

        public void CancelShoot(){
            if(_localCharacter == null) return;
            _networkRopeGun.CancelRope();
        }

    }
    
}
