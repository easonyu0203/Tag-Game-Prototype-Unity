using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ultilities;
using Player;
using System;

namespace Networking{

    public class ServerOnlyData : MonoSingleton<ServerOnlyData>
    {
        /// <summary>
        /// Dictionary of client id to it's PlayerData
        /// </summary>
        /// <typeparam name="ulong">clientId</typeparam>
        /// <typeparam name="PlayerData">playerData</typeparam>

        public MyDictionary <ulong, PlayerData> PlayerData_dic = new MyDictionary<ulong, PlayerData>(); 
        public int ClientCount{get => PlayerData_dic.Count();}

        private void Start() {


            NetworkEvent.Instance.ServerEvent_OnClientDisconnect += PlayerDataDic_HandleDisconnect;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(NetworkEvent.Instance){
                NetworkEvent.Instance.ServerEvent_OnClientDisconnect -= PlayerDataDic_HandleDisconnect;

            }
        }

        private void PlayerDataDic_HandleDisconnect(ulong clientId)
        {
            PlayerData_dic.Remove(clientId);
        }
    }


}
