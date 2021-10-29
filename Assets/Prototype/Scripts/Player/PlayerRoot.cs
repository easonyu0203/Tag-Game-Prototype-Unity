using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;
using MLAPI;

namespace Player{

    public class PlayerRoot : NetworkBehaviour
    {

        /// <summary>
        /// contain All the playerRoot object
        /// </summary>
        static public List<PlayerRoot> PlayerRoot_list = new List<PlayerRoot>();

        /// <summary>
        /// Count of PlayerRoot object
        /// </summary>
        static public int Count => PlayerRoot_list.Count;
        static public event Action<ulong> OnPlayerRootAdd;
        static public event Action<ulong> OnPlayerRootRemove;


        private PlayerCredential _playerCredential;

        public void Initialize(ulong clientId, Networking.ConnectionData connectionData) {
            var playerData_list = GetComponents<IPlayerData>();
            foreach(IPlayerData pData in playerData_list){
                pData.SetClientId(clientId);
            }

            //init credential
            _playerCredential = GetComponent<PlayerCredential>();
            if(_playerCredential == null) Debug.LogError("[PlayerRoot] Cant't get Credential");
            _playerCredential.Name = connectionData.Name;
        }

        private void Awake() {
            PlayerRoot_list.Add(this);
        }

        public override void NetworkStart() {
            Debug.Log($"[PlayerRoot] Add {OwnerClientId}");
            OnPlayerRootAdd?.Invoke(OwnerClientId);
        }

        private void OnDestroy() {
            PlayerRoot_list.Remove(this);
            OnPlayerRootRemove?.Invoke(OwnerClientId);
        }


    }
}
