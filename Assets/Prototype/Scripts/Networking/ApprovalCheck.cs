using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;
using MLAPI;

namespace Networking{
    public class ApprovalCheck : MonoSingleton<ApprovalCheck>
    {
        [SerializeField] Config.NetworkSetting _networkSetting;
        [SerializeField] GameObject _playerRoot;

        /// <summary>
        /// Logic of whether let this client to connect
        /// </summary>
        private bool ApprovalChecking(ConnectionData connectionData){
            Debug.Log("Approval checking");
            // check if can fit more client
            if(NetworkManager.Singleton.ConnectedClientsList.Count >= _networkSetting.MaxClientCount){
                return false;
            }

            return true;
        }

        /// <summary>
        /// Spawning and set up Default prefab for client
        /// </summary>
        private void InitialSpawning(ulong ClientId, ConnectionData connectionData){
            Debug.Log("initial spawning");
            GameObject clientPlayerRoot = Instantiate(_playerRoot);
            clientPlayerRoot.GetComponent<Player.PlayerRoot>().Initialize(ClientId, connectionData);
            clientPlayerRoot.GetComponent<NetworkObject>().SpawnAsPlayerObject(ClientId);
        }

        private void Start() {
            NetworkEvent.Instance.ServerEvent_ConnectionApprovalCallback += OnCheckApproval;
        }

        protected override void OnDestroy(){
            base.OnDestroy();

            if(NetworkEvent.Instance != null){
                NetworkEvent.Instance.ServerEvent_ConnectionApprovalCallback -= OnCheckApproval;
            }

        }

        
        private void OnCheckApproval(byte[] payload, ulong ClientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            
            ConnectionData connectionData = ConnectionData.Decode(payload);
            bool approved = ApprovalChecking(connectionData);

            // this callback control client approved or not, create default prefab or not
            callback(false, null, approved, null, null);
            
            if(approved) InitialSpawning(ClientId, connectionData);
        }

    }

}

