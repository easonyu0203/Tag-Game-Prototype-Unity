using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using System;

public class CheckSceneSync : NetworkBehaviour
{
    [Header("BroadCast Channel")]
    [SerializeField] VoidEventChannelSO GamePlaySceneSync;
    int syncCnt = 0;

    public override void NetworkStart()
    {
        if(IsClient){
            haveSyncServerRpc();
        }
    }

    [ServerRpc( RequireOwnership = false )]
    private void haveSyncServerRpc()
    {
        syncCnt++;
        if(NetworkManager.Singleton.ConnectedClientsList.Count == syncCnt){
            Debug.Log("All client have sync scene");
            GamePlaySceneSync.RaiseEvent();
        }
    }
}
