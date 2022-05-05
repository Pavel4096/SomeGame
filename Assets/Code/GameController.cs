using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private Button _closeAndStartButton;
    [SerializeField] private Button _leaveGameButton;
    [SerializeField] private TMP_Text _playersCount;

    private void Awake()
    {
        _leaveGameButton.onClick.AddListener(LeaveGame);
        if(PhotonNetwork.IsMasterClient)
            _closeAndStartButton.onClick.AddListener(CloseAndStart);
        else
            _closeAndStartButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _leaveGameButton.onClick.RemoveAllListeners();
        _closeAndStartButton.onClick.RemoveAllListeners();
    }

    public void UpdatePlayers(Player[] players)
    {
        _playersCount.text = $"Players count: {players.Length}";
    }

    private void LeaveGame()
    {
        Destroy(gameObject);
        PhotonNetwork.LeaveRoom();
    }

    private void CloseAndStart()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Level");
    }
}
