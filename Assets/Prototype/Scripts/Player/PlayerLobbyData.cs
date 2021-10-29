using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;

using MLAPI.NetworkVariable;


namespace Player{

    /// <summary>
    /// The information/state of this player in Lobby
    /// </summary>
    public class PlayerLobbyData : PlayerData<PlayerLobbyData>
    {
        public Game.CharaterEnum CurrentChoosedCharater {
            get => _currentChoosedCharater.Value;
            set {
                CheckOwner();
                _currentChoosedCharater.Value = value;
            }
        }


        public bool IsReady{
            get => _isReady.Value;
            set {
                CheckOwner();
                _isReady.Value = value;
            }
        }

        public bool IsLobbyHost{
            get => _isLobbyHost.Value;
            set {
                if(!IsServer) Debug.LogError("not server try to set lobby host");
                _isLobbyHost.Value = value;
            }
        }


        public NetworkVariable<Game.CharaterEnum> _currentChoosedCharater = new NetworkVariable<Game.CharaterEnum>(
            new NetworkVariableSettings {WritePermission=NetworkVariablePermission.OwnerOnly},
            Game.CharaterEnum.None
        );
        public NetworkVariableBool _isReady = new NetworkVariableBool(
            new NetworkVariableSettings {WritePermission=NetworkVariablePermission.OwnerOnly},
            false
        );
        public NetworkVariableBool _isLobbyHost = new NetworkVariableBool(false);

        private void CheckOwner(){
            if(!IsOwner) Debug.LogError("[PlayerLobbyData] not owner try to set PlayerLobbyData");
        }
    }

}
