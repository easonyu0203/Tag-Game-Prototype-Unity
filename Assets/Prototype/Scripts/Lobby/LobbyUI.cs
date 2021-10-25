using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;
using Networking;
using Player;
using MLAPI;

namespace Lobby
{

    public class LobbyUI : MonoSingleton<LobbyUI>
    {
        [SerializeField] List<PlayerBlock> _playerBlock_list = new List<PlayerBlock>();

        //reference
        private MyDictionary<ulong, Player.PlayerData> _playerData_dic;

        private void Start()
        {
            // Only run at server
            if (NetworkManager.Singleton.IsClient) return;

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
    }
}