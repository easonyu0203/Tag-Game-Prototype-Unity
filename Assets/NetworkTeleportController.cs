using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;


public class NetworkTeleportController : NetworkBehaviour
{
    public void Teleport(Vector3 position)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{OwnerClientId}
            }
        };
        HandleTeleportClientRpc(position, clientRpcParams);
        
    }

    [ClientRpc]
    private void HandleTeleportClientRpc(Vector3 position, ClientRpcParams clientRpcParams){
        transform.position = position;
    }
}
