using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using Networking;
using Player;
using System;

namespace Game{
    public class NetSpawnCharacter : NetworkBehaviour
    {
        
        [SerializeField] GameObject _ghostPrefab;
        [SerializeField] GameObject _humanPrefab;
        [SerializeField] List<Transform> _ghostSpawnPoints;
        [SerializeField] List<Transform> _HumanSpawnPoints;

        [Header("Listening Channels")]
        [SerializeField] VoidEventChannelSO GamePlaySceneSync;

        private void OnEnable() {
            GamePlaySceneSync.OnEventRaised += OnGamePlaySceneSync;
        }

        private void OnGamePlaySceneSync()
        {
            Debug.Log("Spawning Characters");
            SpawnCharacter();
        }

        private void OnDisable() {
            GamePlaySceneSync.OnEventRaised -= OnGamePlaySceneSync;
        }

        private void SpawnCharacter(){
            int i = 0;
            int j = 0;
            foreach (var pair in PlayerLobbyData.Dic)
            {
                GameObject cha;
                if(pair.Value.CurrentChoosedCharater == CharaterEnum.Human){
                    cha = Instantiate(_humanPrefab, _HumanSpawnPoints[i].position, _humanPrefab.transform.rotation); i++;
                }
                else{
                    cha = Instantiate(_ghostPrefab, _ghostSpawnPoints[i].position, _ghostPrefab.transform.rotation); j++;
                }
                cha.GetComponent<NetworkObject>().SpawnAsPlayerObject(pair.Key);

            }

        }
    }

}

