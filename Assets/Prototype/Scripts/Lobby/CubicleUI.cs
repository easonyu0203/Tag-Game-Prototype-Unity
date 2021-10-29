using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

            // call change manaully when server
            // if(IsServer) OnClientIdChange(id, id);
        }

        public StateEnum State => _state;
        public ulong AssignedClientId => _assignedClientId.Value;

        [SerializeField]
        private NetworkVariableULong _assignedClientId = new NetworkVariableULong(0);
        [SerializeField]
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
                    Task.Run(async()=>{
                        await DisplayPlayer_StateConfigAsync();
                    });
                    break;
            }
        }

        private async Task DisplayPlayer_StateConfigAsync()
        {
            Debug.Log("Async handle display player");

            // if playerRoot of this id doesn't exit yet, wait
            bool haveId = false;
            foreach(PlayerRoot pRoot in PlayerRoot.PlayerRoot_list){
                if(pRoot.OwnerClientId == 0) continue;
                if(pRoot.OwnerClientId == AssignedClientId){
                    haveId = true;
                    break;
                }
            }
            if(!haveId){
                Debug.LogWarning($"Dont have client {AssignedClientId}. block");
                //block thread untill have this id
                SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
                Action<ulong> OnAdd = null;
                OnAdd = (id) => {
                    if(id == AssignedClientId){
                        semaphore.Release(1);
                        PlayerRoot.OnPlayerRootAdd -= OnAdd;
                    }
                };
                PlayerRoot.OnPlayerRootAdd += OnAdd;
                semaphore.Wait(2000);
                Debug.LogWarning($"Get client {AssignedClientId}. Unblock");
            }

            PlayerLobbyData LobbyData = await PlayerLobbyData.GetPlayerDataAsync(AssignedClientId);
            // PlayerCredential credential = await PlayerCredential.GetPlayerDataAsync(AssignedClientId);

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
            PlayerLobbyData.OnPlayerDataRemove += OnPlayerDataRemove;
            ChangeState(StateEnum.NotSyncYet);
        }

        /// <summary>
        /// when playerRemove check if to change state
        /// </summary>
        private void OnPlayerDataRemove(ulong id)
        {
            if(IsClient) return;
            if(State != StateEnum.DisplayPlayer) return;
            if(AssignedClientId == id){
                _assignedClientId.Value = 0;
                Debug.Log($"client {id} destroy, change waiting player");
                ChangeState(StateEnum.WaitingPlayer);
            }
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
            if(newValue == 0) {
                ChangeState(StateEnum.WaitingPlayer);
                return;
            }

            // check id exist
            // Task.Run(async ()=>{
            //     if(await PlayerCredential.ContainClientIdAsync(newValue) == false)
            //         Debug.LogError($"[Cubicle] dont have client {newValue} Credential");
            //     if(await PlayerLobbyData.ContainClientIdAsync(newValue) == false)
            //         Debug.LogError($"[Cubicle] dont have client {newValue} LobbyData");
            // });

            ChangeState(StateEnum.DisplayPlayer);
        }
    }

}
