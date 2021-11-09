using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerDebuger : NetworkBehaviour
{
    [SerializeField] GameObjectEventChannelSO LocalPlayerReadyEvent;

    public override void NetworkStart()
    {
        LocalPlayerReadyEvent.RaiseEvent(gameObject);
    }
}
