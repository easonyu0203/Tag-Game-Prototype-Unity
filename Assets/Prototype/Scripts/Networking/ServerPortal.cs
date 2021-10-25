using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement;
using Ultilities;
using System;
using System.Reflection;
using MLAPI;
using MLAPI.SceneManagement;

namespace Networking{

    /// <summary>
    /// Kinda like initialization of server
    /// </summary>
    public class ServerPortal : MonoSingleton<ServerPortal>
    {
        [Header("Setting")]
        public int MaxPlayerCount = 4;
        [Header("Default Prefab")]
        [Tooltip("Spawn PlayerRoot for Client when client connect")]
        [SerializeField] private GameObject _playerRoot;
        [Tooltip("parent transform to store all player root")]
        [SerializeField] private Transform _playerRootsTransform;

        //reference
        private NetworkEvent _networkEvent;

        protected override void Awake()
        {
            base.Awake();

            _networkEvent = GetComponent<NetworkEvent>();
        }

        private void Start() {
            _networkEvent.ServerEvent_OnServerNetworkReady += OnServerReady;

            _networkEvent.ServerEvent_ConnectionApprovalCallback += ApprovalCheck;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _networkEvent.ServerEvent_OnServerNetworkReady -= OnServerReady;
        }

        private void OnServerReady()
        {

            Debug.Log("[ServerPortal] On Server Network Ready");
            Debug.Log("Load Lobby Scence");

            NetworkSceneManager.SwitchScene("LobbyScene");
        }

        private void ApprovalCheck(byte[] payload, ulong ClientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            
            ConnectionData connectionData = ConnectionData.Decode(payload);
            bool approved = true;

                
            // Chech room is full
            if(ServerOnlyData.Instance.ClientCount >= MaxPlayerCount){
                Debug.Log($"client {ClientId} want to connect but lobby full");
                approved = false;
            }
            else{

                //spawn Default Player
                Action<ulong> ClientConnectCallBack = null;
                ClientConnectCallBack = (ulong clientId) => {

                    if(clientId == ClientId){
                        GameObject clientPlayerRoot = Instantiate(_playerRoot, _playerRootsTransform);
                        clientPlayerRoot.GetComponent<NetworkObject>().SpawnAsPlayerObject(ClientId);
                        clientPlayerRoot.GetComponent<Player.PlayerData>().InitByConnectionData(connectionData);

                        // register to serverOnlyData
                        ServerOnlyData.Instance.PlayerData_dic.Add(ClientId, clientPlayerRoot.GetComponent<Player.PlayerData>());

                        //unsubcribe itself, this only need to be call once
                        _networkEvent.ServerEvent_OnClientConnect -= ClientConnectCallBack;
                    }
                };
                _networkEvent.ServerEvent_OnClientConnect += ClientConnectCallBack;

            }

            callback(false, null, approved, null, null);
        }

        public void StartServer(){
            NetworkManager.Singleton.StartServer();
        }
    }

}
