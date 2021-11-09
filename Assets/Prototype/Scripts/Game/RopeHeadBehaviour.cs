using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class RopeHeadBehaviour : MonoBehaviour
{


    private int _defaultLayer, _playerLayer;
    [HideInInspector]
    public GameObject OwnerCharacter;
    [HideInInspector]
    public RopeBehaviour RopeBehaviour;

    private void Awake() {
        _defaultLayer = LayerMask.NameToLayer("Default");
        _playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other) {    
        if(NetworkManager.Singleton.IsServer == false) return;

        if(other.gameObject.layer == _playerLayer){
            if(other.gameObject.tag == "Human" && OwnerCharacter.tag == "Ghost"){
                Debug.Log("Ghost catch Human");
                RopeBehaviour.GhostCatchHuman(other.gameObject );
            }
        }
        else if(other.gameObject.layer == _defaultLayer){
            Debug.Log("hit wall");
            //hit wall
            RopeBehaviour.OnHitWall();
            GetComponent<SphereCollider>().enabled = false;
        }
    }
}
