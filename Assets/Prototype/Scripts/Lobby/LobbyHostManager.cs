using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

using Player;
using Ultilities;

/// <summary>
/// Run on Server Control how is the lobby host
/// </summary>
public class LobbyHostManager : NetworkBehaviour
{

    
    private ulong? _lobbyHostId = null;
    PlayerLobbyData _playerLobbyData = null;

    public override void NetworkStart()
    {
        if(IsClient) return;
        PlayerRoot.OnPlayerRootAdd += OnAdd;
        PlayerRoot.OnPlayerRootRemove += OnRemove;
    }

    private void OnRemove(ulong id)
    {
        if(_lobbyHostId.HasValue == false) Debug.LogError("remove a player but there is no host");
        if(_lobbyHostId.Value == id){
            Debug.Log($"Host {id} is leaving");
            ulong? canId = null;
            foreach(PlayerRoot pRoot in PlayerRoot.PlayerRoot_list){
                if(pRoot.OwnerClientId == 0){
                    Debug.LogError("PlayerRoot Own by server, may not run network start yet");
                    continue;
                }
                if(pRoot.OwnerClientId != id){
                    //find
                    canId = pRoot.OwnerClientId;
                    break;
                }
            } 
            _lobbyHostId = null;
            _playerLobbyData = null;
            if(canId.HasValue){
                Debug.Log($"Find new Host {canId.Value}");
                // let canId be Host
                Task.Run(async () => {
                    _playerLobbyData = await PlayerLobbyData.GetPlayerDataAsync(canId.Value);
                    _lobbyHostId = canId.Value;
                    Debug.Log("Remove Have find data");
                });

                StartCoroutine(RemoveCoroutine());
            }
        }
    }

    private IEnumerator RemoveCoroutine(){
        if(_playerLobbyData != null){
            Debug.Log("Remove coroutine");
            //wait till have lobby data
            while(_playerLobbyData == null || _lobbyHostId.HasValue == false){
                Debug.LogWarning($"Busy waitng for playerdata {_playerLobbyData}, {_lobbyHostId.HasValue}");
                yield return new WaitForSecondsRealtime(0.1f);
            }
            Debug.Log($"give Host to {_lobbyHostId.Value}");
            _playerLobbyData.IsLobbyHost = true;
        }
    }


    private void OnAdd(ulong id)
    {
        if(_lobbyHostId.HasValue == true) return;

        Task.Run(async () => {
            _playerLobbyData = await PlayerLobbyData.GetPlayerDataAsync(id);
            _lobbyHostId = id;
        });

        StartCoroutine(AddCoroutine());
    }

    private IEnumerator AddCoroutine(){
        //wait till have lobby data
        while(_playerLobbyData == null || _lobbyHostId.HasValue == false){
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Debug.Log($"give Host to {_lobbyHostId.Value}");
        _playerLobbyData.IsLobbyHost = true;

    }
}
