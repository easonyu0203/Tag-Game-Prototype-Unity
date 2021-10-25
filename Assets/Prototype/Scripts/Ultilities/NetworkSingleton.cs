using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

namespace Ultilities{

    /// <summary>
    /// NetworkBehaviour Singleton class
    /// </summary>
    /// <typeparam name="T"> Shoud be same as child class</typeparam>
    public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                // if (_instance == null) Debug.LogError("[NetworkSingleton] Instance is null,didn't create a instance yet.");
                return _instance;
            }
        }

        protected virtual void Awake()
        {

            if (_instance == null)
            {
                _instance = (T)this;
            }
            else if (_instance != (T)this)
            {
                // Debug.LogError("[NetworkSingleton] Try to instantiate multiple singleton object");
                // Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == (T)this)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
