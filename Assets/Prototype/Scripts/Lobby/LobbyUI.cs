using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;
using Networking;
using Player;
using MLAPI;
using UnityEngine.UI;
using System;

namespace Lobby
{

    public class LobbyUI : MonoSingleton<LobbyUI>
    {
        [Header("Reference")]
        [SerializeField] List<PlayerBlock> _playerBlock_list = new List<PlayerBlock>();
        [SerializeField] private Image _ghostButton;
        [SerializeField] private Image _humanButton;
        [SerializeField] private Image _startButton;
        [SerializeField] private Image _isReadyButton;

        //reference
        private MyDictionary<ulong, Player.PlayerData> _playerData_dic;
        private PlayerData _playerData;
        private PlayerBlock _playerBlock;

        private void Start()
        {
            // Only run at server
            if (NetworkManager.Singleton.IsServer)
            {
                //get reference
                _playerData_dic = ServerOnlyData.Instance.PlayerData_dic;

                // Hook to PlayerData dic change
                _playerData_dic.OnAdd += _playerData_dic_OnAdd;
                _playerData_dic.OnRemove += _playerData_dic_OnRemove;

                // init playerBlocks
                foreach (var playerData in _playerData_dic)
                {
                    Debug.Log("There is player data in PlayerDataDic at Lobby Start(Strange?)");
                }
            }
            else
            {
                // is Client
                PlayerData.OnLocalPlayerDataReady += (PlayerData pData) =>
                {
                    // set up reference
                    _playerData = pData;
                    foreach(var p in _playerBlock_list)
                    {
                        if(p.State.Value == PlayerBlock.StateEnum.DisplayPlayer)
                        {
                            if(p.DisplayPlayerData == _playerData)
                            {
                                _playerBlock = p;
                            }
                        }
                    }

                    // hook when tagType change -> change button/background color
                    _playerData.TagType.OnValueChanged += OnTypeChange;
                    _playerData.isReady.OnValueChanged += OnIsReadyChange;
                };
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_playerData)
            {
                _playerData.TagType.OnValueChanged -= OnTypeChange;
                _playerData.isReady.OnValueChanged -= OnIsReadyChange;
            }
        }

        private void OnTypeChange(TagTypeEnum previousValue, TagTypeEnum newValue)
        {
            if(newValue == TagTypeEnum.Ghost)
            {
                _ghostButton.color = Color.red;
                _humanButton.color = Color.white;
            }
            else
            {
                _ghostButton.color = Color.white;
                _humanButton.color = Color.blue;
            }
        }

        private void OnIsReadyChange(bool previousValue, bool newValue)
        {
            if (newValue)
            {
                _isReadyButton.color = Color.yellow;
            }
            else
            {
                _isReadyButton.color = Color.white;
            }
        }

        private void _playerData_dic_OnRemove(ulong clientId, PlayerData playerData)
        {
            foreach (var playerBlock in _playerBlock_list)
            {
                if (playerBlock.State.Value == PlayerBlock.StateEnum.WaittingPlayer) continue;

                if (playerBlock.DisplayPlayerData == playerData)
                {
                    playerBlock.UnDisplayPlayer();
                    return;
                }
            }
        }

        private void _playerData_dic_OnAdd(ulong clientId, PlayerData playerData)
        {
            foreach (var playerBlock in _playerBlock_list)
            {
                if (playerBlock.State.Value == PlayerBlock.StateEnum.WaittingPlayer)
                {
                    playerBlock.DisplayPlayer(playerData);
                    return;
                }
            }
        }

        // BUTTON

        public void OnGhostButtonClick()
        {
            _playerData.TagType.Value = TagTypeEnum.Ghost;
        }

        public void OnHumanButtonClick()
        {
            _playerData.TagType.Value = TagTypeEnum.Human;
        }

        public void OnIsReadyButtonClick()
        {
            _playerData.isReady.Value = !_playerData.isReady.Value;
        }

        public void OnStartButtonClick()
        {
            Debug.Log("Start Game");
        }
    }
}