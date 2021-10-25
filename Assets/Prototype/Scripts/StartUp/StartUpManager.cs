using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using Networking;

public enum NetworkTypeEnum
{
    None,
    Server,
    Client,
}

public class StartUpManager : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Start networking as Client or Server")]
    public NetworkTypeEnum NetworkType = NetworkTypeEnum.None;

    private NetworkManager netManager;

    IEnumerator Start()
    { 
        // wait for other Mono script start function finish
        yield return null;
        netManager = NetworkManager.Singleton;

        if(!Application.isEditor){
            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mlapi", out string mlapiValue))
            {
                switch (mlapiValue)
                {
                    case "server":
                        NetworkType = NetworkTypeEnum.Server;
                        break;
                    case "client":
                        NetworkType = NetworkTypeEnum.Client;
                        break;
                    case "host":
                        throw new System.Exception("I didn't implement hosting...");
                }
            }
        }

        ToNextLevel();

    }

    /// <summary>
    /// end of Start up Scene
    /// </summary>
    private void ToNextLevel()
    {
        switch(NetworkType)
        {
            case NetworkTypeEnum.None:
                throw new System.Exception("Didn't give netork type (client/server)");
            case NetworkTypeEnum.Server:
                ServerPortal.Instance.StartServer();
                break;
            case NetworkTypeEnum.Client:
                SceneManager.LoadScene("MenuScene");
                break;
        }
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}
