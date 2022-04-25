using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class StartGame : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private TMP_Text _errorText;
    private TMP_Text _loginButtonText;

    private string _gameVersion = "1";
    private const string _connectButtonText = "Connect to Photon";
    private const string _disconnectButtonText = "Disconnect from Photon";
    private readonly Color _disconnectedColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    private readonly Color _connectingColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
    private readonly Color _connectedColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _loginButton.onClick.AddListener(ConnectionButtonClicked);
        _loginButtonText = _loginButton.GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        SetButtonDisconnected("Disconnected");
    }

    private void OnDestroy()
    {
        _loginButton.onClick.RemoveAllListeners();
    }

    private void Connect()
    {
        SetButtonConnectingOrDisconnecting();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = _gameVersion;
    }

    private void Disconnect()
    {
        SetButtonConnectingOrDisconnecting();
        PhotonNetwork.Disconnect();
    }

    private void ConnectionButtonClicked()
    {
        if(PhotonNetwork.IsConnected)
            Disconnect();
        else
            Connect();
    }

    public override void OnConnectedToMaster()
    {
        SetButtonConnected("Connected");
        Debug.Log("Connected");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetButtonDisconnected($"{cause}");
        Debug.Log(cause);
    }

    private void SetButtonConnectingOrDisconnecting()
    {
        _errorText.text = String.Empty;
        _loginButton.interactable = false;
        _loginButtonText.color = _connectingColor;
    }

    private void SetButtonDisconnected(string text)
    {
        _loginButton.interactable = true;
        _loginButtonText.text = _connectButtonText;
        _loginButtonText.color = _disconnectedColor;
        _errorText.text = text;
        _errorText.color = _disconnectedColor;
    }

    private void SetButtonConnected(string text)
    {
        _loginButton.interactable = true;
        _loginButtonText.text = _disconnectButtonText;
        _loginButtonText.color = _connectedColor;
        _errorText.text = text;
        _errorText.color = _connectedColor;
    }
}
