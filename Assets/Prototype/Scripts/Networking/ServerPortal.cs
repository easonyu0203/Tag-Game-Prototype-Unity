using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;
using System;
using MLAPI;
using MLAPI.SceneManagement;

namespace Networking{

    /// <summary>
    /// Kinda like initialization of server
    /// </summary>
    public class ServerPortal : MonoSingleton<ServerPortal>
    {

        //reference
        private NetworkEvent _networkEvent;

        protected override void Awake()
        {
            base.Awake();

            _networkEvent = GetComponent<NetworkEvent>();
        }

        private void Start() {
            _networkEvent.ServerEvent_OnServerNetworkReady += OnServerReady;
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

        public void StartServer(){
            NetworkManager.Singleton.StartServer();
        }
    }

}
