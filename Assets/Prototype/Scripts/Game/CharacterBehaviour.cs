using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class CharacterBehaviour : NetworkBehaviour
{
    [Header("Reference")]
    public GameObject CameraFollowTarget;
    public GameObject GunRotatePoint;
    public GameObject GunShootPoint;

    [Header("Broacast Channels")]
    public GameObjectEventChannelSO LocalCharacterReadyEvent;

    public override void NetworkStart()
    {
        if(IsLocalPlayer){
            Debug.Log("Local character ready event");
            LocalCharacterReadyEvent.RaiseEvent(gameObject);
        }
    }
}
