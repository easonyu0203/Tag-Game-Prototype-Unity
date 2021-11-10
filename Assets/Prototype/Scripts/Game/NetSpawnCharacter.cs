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
        [SerializeField] GameObject _HitFXPrefab;
        [SerializeField] List<Transform> _ghostSpawnPoints;
        [SerializeField] List<Transform> _HumanSpawnPoints;

        [Header("Listening Channels")]
        [SerializeField] VoidEventChannelSO GamePlaySceneSync;
        [SerializeField] GameObjectEventChannelSO ServerCatchHumanEvent;

        private GameState _gameState;

        private void OnEnable() {
            GamePlaySceneSync.OnEventRaised += OnGamePlaySceneSync;
            ServerCatchHumanEvent.OnEventRaised += OnCatchHumanEvent;
        }

        private void Awake() {
            _gameState = GetComponent<GameState>();
        }

        private void OnCatchHumanEvent(GameObject catchedHuman)
        {
            // teleport player to spawn point
            Debug.Log("ReSpawn Catched Human");
            GameObject hitFx = Instantiate(_HitFXPrefab, catchedHuman.transform.position, _HitFXPrefab.transform.rotation);
            hitFx.GetComponent<NetworkObject>().Spawn();
            catchedHuman.GetComponent<NetworkTeleportController>().Teleport(_HumanSpawnPoints[0].position);
        }


        private void OnGamePlaySceneSync()
        {
            Debug.Log("Spawning Characters");
            SpawnCharacter();
        }

        private void OnDisable() {
            GamePlaySceneSync.OnEventRaised -= OnGamePlaySceneSync;
            ServerCatchHumanEvent.OnEventRaised -= OnCatchHumanEvent;
        }

        private void SpawnCharacter(){
            int i = 0;
            int j = 0;
            foreach (var pair in PlayerLobbyData.Dic)
            {
                GameObject cha;
                if(pair.Value.CurrentChoosedCharater == CharaterEnum.Human){
                    cha = Instantiate(_humanPrefab, _HumanSpawnPoints[i].position, _humanPrefab.transform.rotation); i++;
                    _gameState.HumanCnt.Value += 1;
                }
                else{
                    cha = Instantiate(_ghostPrefab, _ghostSpawnPoints[i].position, _ghostPrefab.transform.rotation); j++;
                    _gameState.GhostCnt.Value += 1;
                }
                cha.GetComponent<NetworkObject>().SpawnAsPlayerObject(pair.Key);

            }

        }
    }

}

