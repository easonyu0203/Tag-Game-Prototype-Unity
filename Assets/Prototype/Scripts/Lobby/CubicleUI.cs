using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] TextMeshProUGUI _displayPlayerText;
        [SerializeField] Image _background;

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
                    _testText.text = "Waiting Player...";
                    WaitingPlayer_StateConfig();
                    break;
                case StateEnum.DisplayPlayer:
                    _testText.text = $"Client {AssignedClientId}";
                    _testText.gameObject.SetActive(false);
                    _displayPlayerText.gameObject.SetActive(true);

                    var t = Task.Run(async()=>{
                        await DisplayPlayer_StateConfigAsync();
                    });
                    StartCoroutine(UpDateSnapshot());
                    break;
            }
        }

        private IEnumerator UpDateSnapshot(){
            while(_displayPlayerText.text == ""){
                SnapShot(_credential, _lobbyData);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        PlayerLobbyData _lobbyData;
        PlayerCredential _credential;

        private async Task DisplayPlayer_StateConfigAsync()
        {
            Debug.Log("Handle Display Player");
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

            _lobbyData = await PlayerLobbyData.GetPlayerDataAsync(AssignedClientId);
            _credential = await PlayerCredential.GetPlayerDataAsync(AssignedClientId);

            HookDisplayDataChange(_credential, _lobbyData);
        }

        private void HookDisplayDataChange(PlayerCredential credential, PlayerLobbyData lobbyData)
        {
            credential._name.OnValueChanged += (a1, a2) => SnapShot(credential, lobbyData);
            lobbyData._currentChoosedCharater.OnValueChanged += (a1, a2) => SnapShot(credential, lobbyData);
            lobbyData._isReady.OnValueChanged += (a1, a2) => SnapShot(credential, lobbyData);
            lobbyData._isLobbyHost.OnValueChanged += (a1, a2) => SnapShot(credential, lobbyData);
        }

        private void SnapShot(PlayerCredential credential, PlayerLobbyData lobbyData)
        {
            if(credential == null || lobbyData == null){
                Debug.LogWarning($"[SnapShot] PlayerData {AssignedClientId} is Null");
                return;
            }
            string isReady = (lobbyData.IsReady) ? "True" : "False";
            string isHost = (lobbyData.IsLobbyHost)? "True" : "False";
            string character = "---";
            switch(lobbyData.CurrentChoosedCharater){
                case Game.CharaterEnum.None:
                    character = "haven't Choose";
                    _background.color = Color.white;
                    break;
                case Game.CharaterEnum.Human:
                    character = "Human";
                    _background.color = Color.blue;
                    break;
                case Game.CharaterEnum.Ghost:
                    character = "Ghost";
                    _background.color = Color.red;
                    break;
            }
            string displayText = $"Name:\n{credential.Name}\n\nCharacter:\n{character}\n\nIsReady:\n{isReady}\n\nIsHost:\n{isHost}";
            _displayPlayerText.text = displayText;
            
        }

        private void WaitingPlayer_StateConfig()
        {
            if(_testText == null) return;
            _displayPlayerText.text = "";
            _testText.gameObject.SetActive(true);
            _displayPlayerText.gameObject.SetActive(false);
            _background.color = Color.white;
        }

        private void NotSyncYet_StateConfig()
        {
            if(_testText == null) return;
            _testText.gameObject.SetActive(true);
            _displayPlayerText.gameObject.SetActive(false);
            _background.color = Color.white;
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
