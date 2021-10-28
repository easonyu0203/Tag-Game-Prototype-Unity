using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;
using MLAPI;
using System;

namespace Networking{

    /// <summary>
    /// Kinda like initialization of Client
    /// </summary>
    public class ClientPortal : MonoSingleton<ClientPortal>
    {
        //reference
        private NetworkEvent _networkEvent;

        protected override void Awake()
        {
            base.Awake();

            _networkEvent = GetComponent<NetworkEvent>();
        }

        private void Start() {
            _networkEvent.ClientEvent_OnClientNetworkReady += OnClientReady;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _networkEvent.ClientEvent_OnClientNetworkReady -= OnClientReady;
        }

        private void OnClientReady()
        {
            Debug.Log("[ClientPortal] On Client Network Ready");
            Debug.Log("[ClientPortal] Load Menu Scene(by server)");
        }

        public void StartClient(ConnectionData connectionData){
            NetworkManager.Singleton.NetworkConfig.ConnectionData = ConnectionData.Encode(connectionData);
            NetworkManager.Singleton.StartClient();
        }
    }

}
