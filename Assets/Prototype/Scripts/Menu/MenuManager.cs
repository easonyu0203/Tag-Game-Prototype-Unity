using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ultilities;
using Networking;

namespace Menu{

    public class MenuManager : MonoSingleton<MenuManager>
    {

        public enum StateEnum
        {
            Default,
            Connecting
        }

        public event Action<StateEnum> OnStateChange;

        [Header("State")]
        [SerializeField] TMPro.TMP_InputField nameTMP;

        /// <summary>
        /// State of Menu, only can be default or connecting(after join button press)
        /// </summary>
        private StateEnum _state = StateEnum.Default;
        public StateEnum State{
            get{
                return _state;
            }
            private set{
                _state = value;
                OnStateChange?.Invoke(value);
            }
        }

       public void OnJoinButtonClick(){
           // Change State
            State = StateEnum.Connecting;

           // Connect to server as client
           ClientPortal.Instance.StartClient(new Networking.ConnectionData(nameTMP.text));
       }
    }

}
