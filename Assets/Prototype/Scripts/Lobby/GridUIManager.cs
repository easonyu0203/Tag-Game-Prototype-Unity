using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Networking;
using System;

namespace Lobby{
    public class GridUIManager : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] List<CubicleUI> _cubicle_list;

        private void Start() {
            Player.PlayerCredential.OnPlayerDataAdd += OnCredentailAdd;
        }

        private void OnCredentailAdd(ulong clientId)
        {
            Debug.Log("[Grid Manager] handle Client credentailAdd");
            var cube = FindAvailableCubicle();
            cube.AssignClientId(clientId);
        }

        private CubicleUI FindAvailableCubicle(){
            foreach(var cube in _cubicle_list){
                if(cube.State == CubicleUI.StateEnum.WaitingPlayer){
                    return cube;
                }
            }
            Debug.LogError("[GridManager] Can't find availible cube");
            return null;
        }
    }

}
