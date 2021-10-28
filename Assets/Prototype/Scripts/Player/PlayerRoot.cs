using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;

namespace Player{

    public class PlayerRoot : MonoBehaviour
    {
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


    }
}
