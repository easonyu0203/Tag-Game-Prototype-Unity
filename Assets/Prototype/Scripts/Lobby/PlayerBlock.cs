using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;

namespace Lobby{

    public class PlayerBlock : NetworkBehaviour
    {
        public enum StateEnum
        {
            WaittingPlayer,
            DisplayPlayer
        }

        [SerializeField] private Player.PlayerData _displayPlayerData = null;
        public Player.PlayerData DisplayPlayerData {get => _displayPlayerData;}

        [SerializeField]
        private StateEnum _state = StateEnum.WaittingPlayer;
        public StateEnum State {get => _state; private set{
            OnStateChange();
            _state = value;
        }}

        [Header("reference")]
        [SerializeField] private GameObject _waitForPlayerText;
        [SerializeField] private GameObject _displayPlayerDataUI;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _typeText;
        [SerializeField] private TextMeshProUGUI _isReadyText;

        /// <summary>
        /// set the playerData to Display
        /// </summary>
        /// <param name="playerData">player Data</param>
        public void DisplayPlayer(Player.PlayerData playerData){
            //Change State
            State = StateEnum.DisplayPlayer;

            // set up display reference
            _displayPlayerData = DisplayPlayerData;

            HookDisplayUI();
        }

        private void HookDisplayUI()
        {
            // _displayPlayerDataUI.
        }

        public void UnDisplayPlayer(){
            if(_displayPlayerData == null) Debug.LogError("[PlaerBlock] Already unDisplay player");

            //Change State
            State = StateEnum.WaittingPlayer;

            // set up display reference
            _displayPlayerData = null;
        }

        
        private void OnStateChange()
        {
            switch(State){
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
