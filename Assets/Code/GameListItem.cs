using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class GameListItem : MonoBehaviour
{
    [SerializeField] private Button _enterGame;
    [SerializeField] private TMP_Text _itemText;

    public void SetData(string itemText, string name = null, Action<string> callback = null)
    {
        _itemText.text = itemText;

        if(callback != null && name != null)
        {
            _enterGame.onClick.RemoveAllListeners();
            _enterGame.onClick.AddListener(() => callback(name));
        }
    }

    public void Remove()
    {
        _enterGame.onClick.RemoveAllListeners();
    }
}
