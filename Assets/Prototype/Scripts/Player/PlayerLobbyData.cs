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
                CheckOwner();
                _isLobbyHost.Value = value;
            }
        }


        private NetworkVariable<Game.CharaterEnum> _currentChoosedCharater = new NetworkVariable<Game.CharaterEnum>(
            new NetworkVariableSettings {WritePermission=NetworkVariablePermission.OwnerOnly}
        );
        private NetworkVariableBool _isReady = new NetworkVariableBool(
            new NetworkVariableSettings {WritePermission=NetworkVariablePermission.OwnerOnly}
        );
        private NetworkVariableBool _isLobbyHost = new NetworkVariableBool(
            new NetworkVariableSettings {WritePermission=NetworkVariablePermission.OwnerOnly}
        );

        private void CheckOwner(){
            if(!IsOwner) Debug.LogError("[PlayerLobbyData] not owner try to set PlayerLobbyData");
        }
    }

}
