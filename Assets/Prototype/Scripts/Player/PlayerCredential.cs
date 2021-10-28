using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;

using MLAPI.NetworkVariable;

namespace Player{

    /// <summary>
    /// The basic/important information about player
    /// probably need to use network readibility to dont let other client see other's credentail, but for now is ok
    /// </summary>
    public class PlayerCredential : PlayerData<PlayerCredential>
    {
        private NetworkVariableString _name = new NetworkVariableString("[No Name]");
        public string Name { 
            get {return _name.Value;}
            set {
                if(IsClient) Debug.LogError("[PlayerCredential] client try to set name");
                _name.Value = value;
            }
        }

    }

}
