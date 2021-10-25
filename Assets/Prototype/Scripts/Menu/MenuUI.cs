using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu{

    public class MenuUI : MonoBehaviour
    {
        
        [SerializeField]private GameObject _joinButton;
        [SerializeField]private GameObject _ConnectingText;

        private void AtState(MenuManager.StateEnum state){
            switch(state){
                case MenuManager.StateEnum.Default:
                    _joinButton.SetActive(true);
                    _ConnectingText.SetActive(false);
                    break;
                case MenuManager.StateEnum.Connecting:
                    _joinButton.SetActive(false);
                    _ConnectingText.SetActive(true);
                    break;
            }
        }

        private void Start() {
            // listen to state change
            MenuManager.Instance.OnStateChange += OnMenuStateChange;

            // init state
            AtState(MenuManager.Instance.State);
        }

        private void OnDestroy() {
            if(MenuManager.Instance){
                MenuManager.Instance.OnStateChange -= OnMenuStateChange;
            }
        }

        private void OnMenuStateChange(MenuManager.StateEnum newState)
        {
            AtState(newState);
        }
    }

}
