using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using Networking;

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

        /// <summary>
        /// using connection data from approval check to init data
        /// </summary>
        /// <param name="connectionData">connectionData</param>
        public void InitByConnectionData(ConnectionData connectionData){
            Name.Value = connectionData.Name;
        }
        
    }

}
