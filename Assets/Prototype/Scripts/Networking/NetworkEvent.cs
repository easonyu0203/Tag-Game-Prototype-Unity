using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using Ultilities;

namespace Networking{

    /// <summary>
    /// Invoke event that is about networking
    /// </summary>
    public class NetworkEvent : MonoSingleton<NetworkEvent>
    {

        // Event that server want to listen
        /// <summary>
        /// Invoke When server network ready, only invoke on server
        /// </summary>
        public event Action ServerEvent_OnServerNetworkReady;

        /// <summary>
        /// Invoke when a client connect to server, only invoke on server
        /// pass the connected client's clientId 
        /// </summary>
        public event Action<ulong> ServerEvent_OnClientConnect; 

        /// <summary>
        /// Invoke when a client disconnect to server, only invoke on server
        /// pass the connected client's clientId 
        /// </summary>
        public event Action<ulong> ServerEvent_OnClientDisconnect;

        /// <summary>
        /// when client want to connect, we can decide whether to approve or not, if approve then client connect
        /// </summary>
        public event Action<byte[], ulong, NetworkManager.ConnectionApprovedDelegate> ServerEvent_ConnectionApprovalCallback;


        // Event that client want to listen
        /// <summary>
        /// Invoke when client network ready, only invoke on client
        /// </summary>
        public event Action ClientEvent_OnClientNetworkReady;

        
        private NetworkManager _netMananger;

        private void Start() {
            // get referecne of NetworkManager
            _netMananger = NetworkManager.Singleton;

            // listen to NetworkManager's event
            _netMananger.OnServerStarted += OnServerStarted;
            _netMananger.OnClientConnectedCallback += OnClientConnectedCallback;
            _netMananger.OnClientDisconnectCallback += OnClientDisconnectCallback;
            _netMananger.ConnectionApprovalCallback += OnConectionApproval;

        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if(_netMananger){
                _netMananger.OnServerStarted -= OnServerStarted;
                _netMananger.OnClientConnectedCallback -= OnClientConnectedCallback;
                _netMananger.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                _netMananger.ConnectionApprovalCallback -= OnConectionApproval;
            }
        }

        private void OnConectionApproval(byte[] arg1, ulong arg2, NetworkManager.ConnectionApprovedDelegate arg3)
        {
            ServerEvent_ConnectionApprovalCallback?.Invoke(arg1, arg2, arg3);
        }

        void OnServerStarted(){
            ServerEvent_OnServerNetworkReady?.Invoke();


            // Host want have OnClientConnectedCallBack, hence we need to manaully invoke client side event
            if(_netMananger.IsHost){
                ClientEvent_OnClientNetworkReady?.Invoke();
                ServerEvent_OnClientConnect?.Invoke(_netMananger.LocalClientId);
            }
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            if(_netMananger.IsServer){
                ServerEvent_OnClientConnect?.Invoke(clientId);
            }
            else{
                ClientEvent_OnClientNetworkReady?.Invoke();
            }
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            if(_netMananger.IsServer){
                ServerEvent_OnClientDisconnect?.Invoke(clientId);
            }
        }

        

    }

}
