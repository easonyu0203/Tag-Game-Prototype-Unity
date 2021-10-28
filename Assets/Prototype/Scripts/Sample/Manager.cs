using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;

public class Manager : NetworkBehaviour
{
    public GameObject TestPrefab;

    private void Start() {
            if(!Application.isEditor){
            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mlapi", out string mlapiValue))
            {
                switch (mlapiValue)
                {
                    case "server":
                        NetworkManager.Singleton.StartServer();
                        break;
                    case "client":
                        NetworkManager.Singleton.StartClient();
                        break;
                    case "host":
                        throw new System.Exception("I didn't implement hosting...");
                }
            }
        }
    }

    public override void NetworkStart()
    {
        if(NetworkManager.Singleton.IsClient) return;

        GameObject p1 = Instantiate(TestPrefab);
        p1.GetComponent<TestPrefab>().testInt.Value = 100;
        p1.GetComponent<NetworkObject>().Spawn();
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
