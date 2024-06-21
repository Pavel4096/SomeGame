using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public sealed class PlayerData : MonoBehaviour
{
    public float MaxHP => _maxHP;
    public PlayerInformation PlayerInformation;

    private float _maxHP;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), ProcessTitleData, LogError);
    }

    private void ProcessTitleData(GetTitleDataResult result)
    {
        if(!Single.TryParse(result.Data["MaxHP"], out float hp))
            hp = 500.0f;
        _maxHP = hp;
    }

    private void LogError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void Start()
    {

    }
}
