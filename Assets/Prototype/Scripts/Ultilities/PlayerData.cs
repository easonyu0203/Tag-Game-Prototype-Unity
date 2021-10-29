using System;
using System.Threading;
using System.Threading.Tasks;
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
        /// Get playerData by clientId, this is a async call
        /// </summary
        /// <param name="clientId">clientId</param>
        /// <returns>playerData</returns>
        static async public Task<T> GetPlayerDataAsync(ulong clientId){
            if(clientId == 0) throw new System.Exception("want get data fo client 0");
            return await Task<T>.Run(()=>{
                if(Synchronizing){
                    // Block still dic sync and then get playerdata
                    Debug.LogWarning($"[{typeof(T)}] dic is syncing...");
                    SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
                    Action onDoneSync = null;
                    onDoneSync = () => {
                        semaphore.Release(1);
                        DoneSync -= onDoneSync;
                    };
                    DoneSync += onDoneSync;
                    semaphore.Wait(2000);
                    while(Synchronizing){
                        Thread.Sleep(1000);
                        Debug.LogWarning($"[{typeof(T)}] Busy waiting on PlayerData");
                    }
                    Debug.LogWarning($"[{typeof(T)}] dic finish syncing");
                }

                //get value
                T _out;
                while(_dic.TryGetValue(clientId, out _out) == false){
                    Debug.LogWarning($"[{typeof(T)}] can't get {clientId} PlayerData from dic");
                    Debug.LogWarning($"[{typeof(T)}] Busy waiting on PlayerData");
                    Thread.Sleep(100);
                }
                return _out;
            });
        }


        static async public Task<bool> ContainClientIdAsync(ulong id){
            var pData = await GetPlayerDataAsync(id);
            return ( pData != null);
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
        static public bool Synchronizing  {
            get {
                if (unSyncCount != 0){
                    return true;
                }
                return false;
            }
        }
        static private event Action DoneSync; 
        static private int unSyncCount = 0;
        protected NetworkVariableULong _clientId = new NetworkVariableULong(new NetworkVariableSettings{SendTickrate=-1},0);

        protected virtual void Awake() {
            // just spawn this playerData, need time to connect Network
            unSyncCount--;
        }

        public override void NetworkStart()
        {
            // this PlayerData have sync, add to dic
            if(ClientId == 0) Debug.LogError($"[{typeof(T)}, {OwnerClientId}] try to add clientId 0 to dic");
            _dic.Add(_clientId.Value, (T)this);
            Debug.Log($"[{typeof(T)}] add {_clientId.Value}");
            unSyncCount++;


            // check is dic sync, if so invoke event
            if(Synchronizing == false){
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

            Debug.Log($"[{typeof(T)}] remove {_clientId.Value}");
            // invoke ondestroy event
            OnPlayerDataRemove?.Invoke(ClientId);
        }
    }

}

