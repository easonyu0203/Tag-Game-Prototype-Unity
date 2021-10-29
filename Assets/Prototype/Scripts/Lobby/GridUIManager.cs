using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

using Networking;
using System;

namespace Lobby{
    public class GridUIManager : NetworkBehaviour
    {
        [Header("Reference")]
        [SerializeField] List<CubicleUI> _cubicle_list;

        private void Start() {
            if(!IsServer) return;
            Player.PlayerRoot.OnPlayerRootAdd += OnPlyaerRootAdd;
        }

        private void OnPlyaerRootAdd(ulong clientId)
        {
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
