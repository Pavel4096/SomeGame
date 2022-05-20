using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class ShopItem : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _price;

    public void SetData(string title, string price, Action clickCallback)
    {
        _title.text = title;
        _price.text = price;
        _button.onClick.AddListener(() => clickCallback());
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
