using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameClient : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameList _gameList;
    [SerializeField] private NewGameWindow _newGameWindow;
    [SerializeField] private ShopWindow _shopWindow;
    [SerializeField] private InventoryWindow _inventoryWindow;
    [SerializeField] private GameController _gameControllerPrefab;
    [SerializeField] private CharactersWindow _charactersWindow;
    [SerializeField] private NewCharacterWindow _newCharacterWindow;
    private GameController _gameController;

    private const byte _maxPlayersCount = 5;

    private void Awake()
    {
        Connect();
    }

    private void Start()
    {
        _newGameWindow.CreateNewGame += CreateRoom;
        _newGameWindow.JoinGame += JoinExisting;
        _gameList.JoinGame += JoinExisting;
    }

    private void OnDestroy()
    {
        _newGameWindow.CreateNewGame -= CreateRoom;
        _newGameWindow.JoinGame -= JoinExisting;
        _gameList.JoinGame -= JoinExisting;
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _gameList.gameObject.SetActive(true);
        _newGameWindow.gameObject.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _gameList.UpdateGames(roomList);
    }

    public override void OnJoinedRoom()
    {
        _gameList.gameObject.SetActive(false);
        _newGameWindow.gameObject.SetActive(false);
        _shopWindow.gameObject.SetActive(false);
        _inventoryWindow.gameObject.SetActive(false);
        _charactersWindow.gameObject.SetActive(false);
        _newCharacterWindow.gameObject.SetActive(false);
        _gameController = Instantiate(_gameControllerPrefab);
        _gameController.UpdatePlayers(PhotonNetwork.PlayerList);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        _gameController.UpdatePlayers(PhotonNetwork.PlayerList);
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        _gameController.UpdatePlayers(PhotonNetwork.PlayerList);
    }

    public override void OnCreateRoomFailed(short returnCode, string error)
    {
        Debug.Log($"{returnCode} - {error}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
    }

    private void CreateRoom(string name = null, bool isVisible = true)
    {
        if(name == null)
            name = $"Game {Random.Range(0, 1000000)}";
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = _maxPlayersCount;
        roomOptions.EmptyRoomTtl = 5000;
        roomOptions.IsVisible = isVisible;
        PhotonNetwork.CreateRoom(name, roomOptions);
    }

    private void JoinExisting(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }
}
