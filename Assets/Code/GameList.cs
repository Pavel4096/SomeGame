using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameList : MonoBehaviour
{
    [SerializeField] Transform _gameListRoot;
    [SerializeField] GameObject _gameListItem;

    public event Action<string> JoinGame;

    private GameListItem _gameListItemPrefab;
    private Dictionary<string, RoomInfo> _games = new Dictionary<string, RoomInfo>();
    private List<GameListItem> _items = new List<GameListItem>();

    private void Awake()
    {
        _gameListItemPrefab = _gameListItem.GetComponent<GameListItem>();
    }

    public void UpdateGames(List<RoomInfo> roomList)
    {
        for(var i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList)
                _games.Remove(roomList[i].Name);
            else
                _games[roomList[i].Name] = roomList[i];
        }

        if(roomList.Count > 0)
            ShowGames();
    }

    private void ShowGames()
    {
        for(var i = 0; i < _gameListRoot.childCount; i++)
        {
            Destroy(_gameListRoot.GetChild(i).gameObject);
            _items[i].Remove();
        }
        
        foreach(var currentGame in _games)
        {
            GameListItem currentItem = Instantiate(_gameListItemPrefab, _gameListRoot);
            currentItem.SetData(currentGame.Key, currentGame.Key, (string name) => JoinGame?.Invoke(name));
            _items.Add(currentItem);
        }
    }
}
