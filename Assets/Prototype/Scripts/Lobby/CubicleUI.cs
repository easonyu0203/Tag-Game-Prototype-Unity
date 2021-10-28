using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using MLAPI;
using MLAPI.NetworkVariable;

using Player;
using System;

namespace Lobby{

    /// <summary>
    /// display playerData or waiting player, contain all the logic between change between this UI state
    /// </summary>
    public class CubicleUI : NetworkBehaviour
    {
        [Header("Reference")]
        [SerializeField] TextMeshProUGUI _testText;

        /// <summary>
        /// Assign a clientId to this, 
        /// this class with handle the logic of whether id is valid etc.
        /// should only call on server
        /// </summary>
        /// <param name="id">clientId</param>
        public void AssignClientId(ulong id){
            _assignedClientId.Value = id;
        }

        public StateEnum State => _state;
        public ulong AssignedClientId => _assignedClientId.Value;

        private NetworkVariableULong _assignedClientId = new NetworkVariableULong(0);
        private StateEnum _state = StateEnum.None;
        public enum StateEnum{
            None,
            NotSyncYet,
            WaitingPlayer,
            DisplayPlayer
        }

        /// <summary>
        /// this is Dirty change, mean if is same as last state, this method still valid
        /// Call when id change, awake, or playerData Destroy
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeState(StateEnum newState){
            // Change State
            _state = newState;

            // state logic
            switch(newState){
                case StateEnum.None:
                    Debug.LogError("[Cubicle] Change to None State, What?!");
                    break;
                case StateEnum.NotSyncYet:
                    _testText.text = "NotSyncYet";
                    NotSyncYet_StateConfig();
                    break;
                case StateEnum.WaitingPlayer:
                    _testText.text = "Waiting Player";
                    WaitingPlayer_StateConfig();
                    break;
                case StateEnum.DisplayPlayer:
                    _testText.text = $"Client {AssignedClientId}";
                    DisplayPlayer_StateConfig();
                    break;
            }
        }

        private void DisplayPlayer_StateConfig()
        {
            Debug.Log("handle display player");

            var LobbyData = PlayerLobbyData.GetPlayerData(AssignedClientId);
            var credential = PlayerCredential.GetPlayerData(AssignedClientId);
            //handle when this client died
            Action<ulong> onPlayerDataDestroy = null;
            ulong currentId = AssignedClientId;
            onPlayerDataDestroy = (clientId) => {
                if(currentId == clientId){
                    if(currentId != AssignedClientId) Debug.LogError("[Cubicle] id have change ? (this I not sure why)");
                    ChangeState(StateEnum.WaitingPlayer);
                }
            };
            PlayerLobbyData.OnPlayerDataRemove += onPlayerDataDestroy;

        }


        private void WaitingPlayer_StateConfig()
        {
            Debug.Log("Handle waiting player");
        }

        private void NotSyncYet_StateConfig()
        {
            Debug.Log("Handle not sync yet");
        }

        private void Awake() {
            _assignedClientId.OnValueChanged += OnClientIdChange;
            ChangeState(StateEnum.NotSyncYet);
        }

        public override void NetworkStart()
        {
            if(State == StateEnum.NotSyncYet){
                // network have connect, change to waiting
                ChangeState(StateEnum.WaitingPlayer);
            }
        }

        private void OnClientIdChange(ulong previousValue, ulong newValue)
        {
            Debug.Log($"[Cubicle] id change {previousValue} -> {newValue}");
            // have realy change, (MLAPI is weird...)
            if(previousValue == newValue) Debug.LogWarning("[Cubicle] id change to same id");
            // check is valid change
            if(newValue == 0) Debug.LogError("Change id to 0, its weird");
            // check id exist
            if(PlayerCredential.ContainClientId(newValue) == false)
                Debug.LogError($"[Cubicle] dont have client {newValue} Credential");
            if(PlayerLobbyData.ContainClientId(newValue) == false)
                Debug.LogError($"[Cubicle] dont have client {newValue} LobbyData");

            ChangeState(StateEnum.DisplayPlayer);
        }
    }

}
