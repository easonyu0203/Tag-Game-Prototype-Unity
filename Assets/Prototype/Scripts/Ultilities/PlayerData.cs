using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

using Ultilities;

namespace Ultilities{

    public interface IPlayerData
    {
        void SetClientId(ulong clientId);
    }

    /// <summary>
    /// Base class for all playerData Type, provide dictionary for id->data, and sync across client
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PlayerData<T> : NetworkBehaviour, IPlayerData where T: PlayerData<T>
    {
        public ulong ClientId => _clientId.Value;

        /// <summary>
        /// Get playerData by clientId, this method will block if playerData NetworkVariable is still syncing, use with caution please
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns>playerData</returns>
        static public T GetPlayerData(ulong clientId){

            if(Synchronizing){
                // Block still dic sync and then get playerdata
                Debug.LogWarning("[PlayerData] dic is syncing...");
                SemaphoreSlim semaphore = new SemaphoreSlim(1);
                Action onDoneSync = null;
                onDoneSync = () => {
                    semaphore.Release();
                    DoneSync -= onDoneSync;
                };
                DoneSync += onDoneSync;
                semaphore.Wait();
                Debug.LogWarning("[PlayerData] dic finish syncing");
            }

            //get value
            T _out;
            if(_dic.TryGetValue(clientId, out _out) == false){
                Debug.LogError($"[PlayerData] can't get {clientId} PlayerData from dic");
            }
            return _out;

        }


        static public bool ContainClientId(ulong id){
            return (GetPlayerData(id) != null);
        }

        /// <summary>
        /// set this playerData to a client
        /// this need to be call on server before network spwan
        /// </summary>
        /// <param name="clientId">clientId</param>
        public void SetClientId(ulong clientId){
            if(IsClient) Debug.LogError("[PlayerData] Client try to set client Id");
            if(_haveSetClientId == true) Debug.LogError("[PlayerData] try to set clientId again");
            _clientId.Value = clientId;
            _haveSetClientId = true;

        }

        static public event Action<ulong> OnPlayerDataRemove;
        static public event Action<ulong> OnPlayerDataAdd;


        private bool _haveSetClientId = false;
        static private MyDictionary<ulong, T> _dic = new MyDictionary<ulong, T>();
        static public bool Synchronizing => unSyncCount != 0;
        static private event Action DoneSync; 
        static private int unSyncCount = 0;
        protected NetworkVariableULong _clientId = new NetworkVariableULong(new NetworkVariableSettings{SendTickrate=-1},0);

        protected virtual void Awake() {
            Debug.Log("[PlayerData] Awake");
            // just spawn this playerData, need time to connect Network
            unSyncCount--;
        }

        public override void NetworkStart()
        {
            Debug.Log("[PlayerData] NetworkStart");
            // this PlayerData have sync, add to dic
            if(ClientId == 0) Debug.LogError("[PlayerData] try to add clientId 0 to dic");
            Debug.Log($"[PlayerData] {(T)this}");
            _dic.Add(_clientId.Value, (T)this);
            unSyncCount++;

            // check is dic sync, if so invoke event
            if(Synchronizing == false){
                Debug.Log("[PlayerData] DoneSync Event");
                DoneSync?.Invoke();
            }

            //invoke add event
            OnPlayerDataAdd?.Invoke(ClientId);
        }

        protected virtual void OnDestroy(){
            // remember to remove this from dic when destroy
            if(!_dic.Remove(_clientId.Value)){
                Debug.LogError($"[PlayerData] cant remove {_clientId.Value} from dic");
            }

            // invoke ondestroy event
            OnPlayerDataRemove?.Invoke(ClientId);
        }
    }

}

