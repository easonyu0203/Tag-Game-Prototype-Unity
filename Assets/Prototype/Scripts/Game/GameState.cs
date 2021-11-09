using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using System;

public class GameState : NetworkBehaviour
{
    [Header("In Game Data")]
    public NetworkVariableInt GhostCnt = new NetworkVariableInt(0);
    public NetworkVariableInt HumanCnt = new NetworkVariableInt(0);
    public NetworkVariableInt CatchedHumanCnt = new NetworkVariableInt(0);
    public int PlayerCnt => GhostCnt.Value + HumanCnt.Value;
    [Header("Listening Channel")]
    public GameObjectEventChannelSO ServerCatchHumanEvent;
    [Header("BroadCast Channel")]
    public IntEventChannelSO CatchedHumanCntChangeEvent;

    private void OnEnable() {
        CatchedHumanCnt.OnValueChanged += OnCatchedHumanCntChange;
        ServerCatchHumanEvent.OnEventRaised += OnCatchHumanEvent;
    }

    private void OnDisable() {
        CatchedHumanCnt.OnValueChanged -= OnCatchedHumanCntChange;
        ServerCatchHumanEvent.OnEventRaised += OnCatchHumanEvent;
    }

    private void OnCatchHumanEvent(GameObject catchedHuman)
    {
        CatchedHumanCnt.Value++;
        Debug.Log($"catchedHumanCnt: {CatchedHumanCnt.Value}");
    }

    private void OnCatchedHumanCntChange(int previousValue, int newValue)
    {
        if(IsServer == false) return;
        CatchedHumanCntChangeEvent.RaiseEvent(newValue);
    }
}
