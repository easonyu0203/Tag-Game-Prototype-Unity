using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using System;

public class NetworkGameUIManager : NetworkBehaviour
{
    public NetworkVariableString displyCatchedCntStr = new NetworkVariableString("");

    [SerializeField] TextMeshProUGUI displayText;

    [Header("Listening Channel")]
    [SerializeField] IntEventChannelSO CatchedHumanCntChangeEvent;

    private void OnEnable() {
        CatchedHumanCntChangeEvent.OnEventRaised += OnCatchedCntChange;
        displyCatchedCntStr.OnValueChanged += CatchedCntChange;
    }

    private void OnDisable() {
        CatchedHumanCntChangeEvent.OnEventRaised -= OnCatchedCntChange;
        displyCatchedCntStr.OnValueChanged -= CatchedCntChange;
    }

    private void CatchedCntChange(string previousValue, string newValue)
    {
        displayText.text = displyCatchedCntStr.Value;
    }

    private void OnCatchedCntChange(int num)
    {
        displyCatchedCntStr.Value = $"Catched Human: {num}";   
    }

    public override void NetworkStart()
    {
        displyCatchedCntStr.Value = $"Catched Human: 0";
    }


}
