using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class NewGameWindow : MonoBehaviour
{
    [SerializeField] private TMP_InputField _gameName;
    [SerializeField] private Toggle _hidden;
    [SerializeField] private Button _createNewGame;
    [SerializeField] private Button _joinGame;

    public event Action<string, bool> CreateNewGame;
    public event Action<string> JoinGame;

    private void Awake()
    {
        _createNewGame.onClick.AddListener(ProcessCreateNewGame);
        _joinGame.onClick.AddListener(ProcessJoinGame);
    }

    private void OnDestroy()
    {
        _createNewGame.onClick.RemoveAllListeners();
        _joinGame.onClick.RemoveAllListeners();
    }

    private void ProcessCreateNewGame()
    {
        GUIUtility.systemCopyBuffer = _gameName.text;
        CreateNewGame?.Invoke(_gameName.text, !_hidden.isOn);
    }

    private void ProcessJoinGame()
    {
        JoinGame?.Invoke(_gameName.text);
    }
}
