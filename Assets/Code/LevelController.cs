using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _leaveGameButton;
    [SerializeField] private string _playerPrefab;
    private GameObject _player;

    private void Awake()
    {
        PlayerData playerData = Object.FindObjectOfType<PlayerData>();
        object[] data = new object[] { playerData.PlayerInformation.MaxHP, playerData.PlayerInformation.Damage };
        Vector3 position = new Vector3(Random.Range(-4.0f, 4.0f), 0.0f, Random.Range(-4.0f, 4.0f));
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up);
        _player = PhotonNetwork.Instantiate(_playerPrefab, position, rotation, 0, data);

        _leaveGameButton.onClick.AddListener(LeaveGame);
    }

    private void OnDestroy()
    {
        _leaveGameButton.onClick.RemoveAllListeners();
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    private void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }
}
