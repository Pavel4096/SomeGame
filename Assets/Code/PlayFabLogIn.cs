using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class PlayFabLogIn : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private TMP_Text _loginText;

    private const string _IdStorageKey = "PlayFabUserGUID";
    private const string _connectingMessage = "Connecting";
    private readonly Color _connectingColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
    private const string _connectedMessage = "Connected";
    private readonly Color _connectedColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    private const string _disconnectedMessage = "Disconnected";
    private readonly Color _disconnectedColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    private string _userGUID;
    private bool _needsSaveGUID;

    private void Awake()
    {
        _loginButton.onClick.AddListener(IdLogIn);
        SetDisconnected();
    }

    private void OnDestroy()
    {
        _loginButton.onClick.RemoveAllListeners();
    }

    private void IdLogIn()
    {
        _needsSaveGUID = !PlayerPrefs.HasKey(_IdStorageKey);
        string newGUID = Guid.NewGuid().ToString();
        _userGUID = PlayerPrefs.GetString(_IdStorageKey, newGUID);
        LoginWithCustomIDRequest loginRequest = new LoginWithCustomIDRequest {
            CreateAccount = _needsSaveGUID,
            CustomId = _userGUID
        };

        SetConnecting();
        PlayFabClientAPI.LoginWithCustomID(loginRequest, LoginSucceeded, LoginFailed);
    }

    private void LoginSucceeded(LoginResult result)
    {
        SetConnected();
        Debug.Log("Connected");
        if(_needsSaveGUID)
            PlayerPrefs.SetString(_IdStorageKey, _userGUID);
    }

    private void LoginFailed(PlayFabError error)
    {
        string errorMessage = error.GenerateErrorReport();
        SetDisconnected(errorMessage);
        Debug.Log($"Login failed: {errorMessage}");
    }

    private void SetDisconnected(string text = null)
    {
        if(String.IsNullOrEmpty(text))
            text = _disconnectedMessage;
        SetTextAndColor(_disconnectedMessage, _disconnectedColor);
    }

    private void SetConnecting()
    {
        SetTextAndColor(_connectingMessage, _connectingColor);
    }

    private void SetConnected()
    {
        SetTextAndColor(_connectedMessage, _connectedColor);
    }

    private void SetTextAndColor(string message, Color color)
    {
        _loginText.text = message;
        _loginText.color = color;
    }
}
