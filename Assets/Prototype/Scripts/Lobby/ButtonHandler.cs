using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using TMPro;

using Player;

public class ButtonHandler : NetworkBehaviour
{
    [SerializeField] Button _ghostButton;
    [SerializeField] Button _humanButton;
    [SerializeField] Button _readyButton;
    [SerializeField] Button _startButton;
    [SerializeField] TextMeshProUGUI _prompText;
    PlayerLobbyData _lobbyData;

    public override void NetworkStart()
    {
        if(IsServer) return;
        // hook button, cause we want button to work after network start
        _ghostButton.onClick.AddListener(OnGhostButton);
        _humanButton.onClick.AddListener(OnHumanButton);
        _readyButton.onClick.AddListener(OnReadyButton);
        _startButton.onClick.AddListener(OnStartButton);
        Task.Run(async () =>{
            _lobbyData = await PlayerLobbyData.GetPlayerDataAsync(NetworkManager.Singleton.LocalClientId);
        });

        StartCoroutine(CanUseStartButton());
    }

    public void OnGhostButton(){
        _ghostButton.GetComponent<Image>().color = Color.red;
        _humanButton.GetComponent<Image>().color = Color.white;

        StartCoroutine(SetCharacterType(Game.CharaterEnum.Ghost));
    }

    public void OnHumanButton(){
        _ghostButton.GetComponent<Image>().color = Color.white;
        _humanButton.GetComponent<Image>().color = Color.blue;

        StartCoroutine(SetCharacterType(Game.CharaterEnum.Human));
    }

    public void OnReadyButton(){
        StartCoroutine(SetIsReady());
    }

    public void OnStartButton(){
        Debug.Log("Start Button Press");
    }

    private IEnumerator CanUseStartButton(){
        while(_lobbyData == null){
            Debug.LogWarning("Busy waiting for lobby data");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        _lobbyData._isLobbyHost.OnValueChanged += (a1,a2) =>{
            if(a2 == true){
                _startButton.gameObject.SetActive(true);
            }
            else{
                _startButton.gameObject.SetActive(false);
            }
        };

        if(_lobbyData.IsLobbyHost){
            _startButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator SetCharacterType(Game.CharaterEnum character){
        while(_lobbyData == null){
            yield return new WaitForSecondsRealtime(0.1f);
            Debug.LogWarning("Busy waiting for lobby data");
        }
        Debug.Log($"Change character {character}");
        _lobbyData.CurrentChoosedCharater = character;
    }

    private IEnumerator SetIsReady(){
        while(_lobbyData == null){
            yield return new WaitForSecondsRealtime(0.1f);
            Debug.LogWarning("Busy waiting for lobby data");
        }
        bool isReady = !_lobbyData.IsReady;
        //check can ready
        if(isReady &&_lobbyData.CurrentChoosedCharater == Game.CharaterEnum.None){
            if(isPromping == true){
                StopCoroutine("Promping");
            }
            StartCoroutine("Promping");
        }
        else{
            Debug.Log($"Change isready {isReady}");
            _lobbyData.IsReady = isReady;
            switch(isReady){
                case true:
                    _readyButton.GetComponent<Image>().color = Color.yellow;
                    break;
                case false:
                    _readyButton.GetComponent<Image>().color = Color.white;
                    break;
            }
        }
    }

    private bool isPromping = false;
    private IEnumerator Promping(){
        isPromping = true;
        _prompText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        _prompText.gameObject.SetActive(false);
        isPromping = false;
    }



}
