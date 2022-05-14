using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelController : MonoBehaviour
{
    [SerializeField] private Button _leaveGameButton;
    [SerializeField] private string _playerPrefab;
    private GameObject _player;

    private void Awake()
    {
        Vector3 position = new Vector3(Random.Range(-4.0f, 4.0f), 0.0f, Random.Range(-4.0f, 4.0f));
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up);
        _player = PhotonNetwork.Instantiate(_playerPrefab, position, rotation);

        _leaveGameButton.onClick.AddListener(LeaveGame);
    }

    private void OnDestroy()
    {
        _leaveGameButton.onClick.RemoveAllListeners();
    }

    private void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
