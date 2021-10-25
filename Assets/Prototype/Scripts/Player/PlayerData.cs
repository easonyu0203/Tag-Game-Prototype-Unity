using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Networking;
using System;

namespace Player{

    /// <summary>
    /// Store this play/client basic data, this will sync to server, all other client can see this too.`
    /// </summary>
    public class PlayerData : NetworkBehaviour
    {
        public NetworkVariableULong ClientId = new NetworkVariableULong(0);
        public NetworkVariableString Name = new NetworkVariableString("[No Name]");
        public NetworkVariable<TagTypeEnum> TagType = new NetworkVariable<TagTypeEnum>(TagTypeEnum.None);
        public NetworkVariableBool IsLobbyHost = new NetworkVariableBool(false);
        public NetworkVariableBool isReady = new NetworkVariableBool(false);

        /// <summary>
        /// using connection data from approval check to init data
        /// </summary>
        /// <param name="connectionData">connectionData</param>
        public void InitByConnectionData(ulong clientId, ConnectionData connectionData){
            ClientId.Value = clientId;
            Name.Value = connectionData.Name;

            //register to dictionary
            _playerData_dic.Add(clientId, this);
        }

        private void Awake()
        {
            if (IsServer)
            {
                OnSetClientId = (a1, a2) => {
                    RegisterDicClientRpc();
                    ClientId.OnValueChanged -= OnSetClientId;
                };

                ClientId.OnValueChanged += OnSetClientId;
            }
        }

        private NetworkVariableULong.OnValueChangedDelegate OnSetClientId = null;

        [ClientRpc]
        public void RegisterDicClientRpc()
        {
            _playerData_dic.Add(ClientId.Value, this);
        }

        static private Dictionary<ulong, PlayerData> _playerData_dic = new Dictionary<ulong, PlayerData>();

        private void OnDestroy()
        {
            // unregister
            _playerData_dic.Remove(this.ClientId.Value);

        }

        static public PlayerData ClientIdToPlayerData(ulong clientId)
        {
            if (!_playerData_dic.ContainsKey(clientId)) throw new System.Exception("Player Data dict dont contain this clinet id");
            return _playerData_dic[clientId];
        }

        static public ulong PlayerdataToClientId(PlayerData playerData)
        {
            return playerData.ClientId.Value;
        }
    }

}
