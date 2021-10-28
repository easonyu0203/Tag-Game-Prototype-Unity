using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.NetworkVariable;
using System;

public class TestPrefab : NetworkBehaviour
{
    public NetworkVariableInt testInt = new NetworkVariableInt(new NetworkVariableSettings{SendTickrate=-1}, 0);

    private void Awake() {
        testInt.OnValueChanged += OnChange;
    }

    private void OnChange(int previousValue, int newValue)
    {
        Debug.Log($"testing value Change: {previousValue}, {newValue}");
    }

    public override void NetworkStart()
    {
        Debug.Log($"TestInt Value: {testInt.Value}");
    }

    private void OnEnable() {
        Debug.Log("enable");
    }
}
