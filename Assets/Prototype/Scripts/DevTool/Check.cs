using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;

public class Check : NetworkBehaviour
{
    public override void NetworkStart()
    {
        CheckServerId();
    }

    private void LogError(string str){
        Debug.LogError($"[Dev Check] {str}");
    }

    private void CheckServerId()
    {
        if(NetworkManager.Singleton.ServerClientId != 0){
            LogError("ServerClientId != 0, the playerData class will break");
        }
    }
}
