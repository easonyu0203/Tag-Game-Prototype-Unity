using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using Player;
using MLAPI.Messaging;

namespace Lobby{

    public class PlayerBlock : NetworkBehaviour
    {
        public enum StateEnum
        {
            WaittingPlayer,
            DisplayPlayer
        }

        [SerializeField] private PlayerData _displayPlayerData = null;
        public PlayerData DisplayPlayerData {get => _displayPlayerData;}

        //[SerializeField] private ulong _displayPlayerClientId;
        //public ulong DisplayPlayerClientId { get => _displayPlayerClientId; }

        //[SerializeField]
        //private StateEnum _state = StateEnum.WaittingPlayer;
        //public StateEnum State {get => _state; private set{
        //    _state = value;
        //    OnStateChange();
        //}}

        public NetworkVariable<StateEnum> State = new NetworkVariable<StateEnum>(StateEnum.WaittingPlayer);

        [Header("reference")]
        [SerializeField] private GameObject _waitForPlayerText;
        [SerializeField] private GameObject _displayPlayerDataUI;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _typeText;
        [SerializeField] private TextMeshProUGUI _isReadyText;

        private void Awake()
        {
            State.OnValueChanged += OnStateChange;

            //init PlayerBlock
            OnStateChange(StateEnum.WaittingPlayer, StateEnum.WaittingPlayer);
        }

        /// <summary>
        /// set the playerData to Display
        /// </summary>
        /// <param name="playerData">player Data</param>
        public void DisplayPlayer(Player.PlayerData playerData){
            //Change State
            State.Value = StateEnum.DisplayPlayer;

            // set up display reference
            _displayPlayerData = playerData;

            HookDisplayUI();
            SnapShotPlayerDataToUI();
        }

        public void UnDisplayPlayer()
        {
            if (_displayPlayerData == null) Debug.LogError("[PlaerBlock] Already unDisplay player");

            UnHookDisplayUI();

            //Change State
            State.Value = StateEnum.WaittingPlayer;

            // set up display reference
            _displayPlayerData = null;


        }

        /// <summary>
        /// listen to change of playerData
        /// </summary>
        private void HookDisplayUI()
        {
            _displayPlayerData.Name.OnValueChanged += OnDataChange1;
            _displayPlayerData.TagType.OnValueChanged += OnDataChange2;
            _displayPlayerData.isReady.OnValueChanged += OnDataChange3;

            UnHookDisplayUI = () => {
                _displayPlayerData.Name.OnValueChanged -= OnDataChange1;
                _displayPlayerData.TagType.OnValueChanged -= OnDataChange2;
                _displayPlayerData.isReady.OnValueChanged -= OnDataChange3;
            };   
        }

        private Action UnHookDisplayUI;


        private void OnDataChange3(bool previousValue, bool newValue)
        {
            SnapShotPlayerDataToUI();
        }

        private void OnDataChange2(TagTypeEnum previousValue, TagTypeEnum newValue)
        {
            SnapShotPlayerDataToUI();
        }

        private void OnDataChange1(string previousValue, string newValue)
        {
            SnapShotPlayerDataToUI();
        }


        /// <summary>
        /// Take a SanpShot of the state of PlayerData and display to UI
        /// </summary>
        private void SnapShotPlayerDataToUI()
        {
            //Server Change
            UpdateUI();

            // tell client to update UI
            if (_displayPlayerData == null) Debug.LogError("display data is null when tell client to display");
            ulong displayId = PlayerData.PlayerdataToClientId(_displayPlayerData);
            SnapShotPlayerDataToUIClientRpc(displayId);
        }

        void UpdateUI()
        {
            _nameText.text = _displayPlayerData.Name.Value;
            switch (_displayPlayerData.TagType.Value)
            {
                case Player.TagTypeEnum.None:
                    _typeText.text = "None";
                    _background.color = Color.white;
                    break;
                case Player.TagTypeEnum.Human:
                    _typeText.text = "Human";
                    _background.color = Color.blue;
                    break;
                case Player.TagTypeEnum.Ghost:
                    _typeText.text = "Ghost";
                    _background.color = Color.red;
                    break;
            }
            _isReadyText.text = _displayPlayerData.isReady.Value ? "True" : "False";

        }

        [ClientRpc]
        void SnapShotPlayerDataToUIClientRpc(ulong displayId)
        {
            // first update display playerData
            _displayPlayerData = PlayerData.ClientIdToPlayerData(displayId);

            //UpdateUI
            UpdateUI();
        }





        private void OnStateChange(StateEnum a1, StateEnum a2)
        {
            switch(State.Value){
                case StateEnum.WaittingPlayer:
                    _waitForPlayerText.SetActive(true);
                    _displayPlayerDataUI.SetActive(false);
                    break;
                case StateEnum.DisplayPlayer:
                    _waitForPlayerText.SetActive(false);
                    _displayPlayerDataUI.SetActive(true);
                    break;
            }
        }
    }

}
