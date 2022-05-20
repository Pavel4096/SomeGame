using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class InventoryItem : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _title;

    public void SetData(string title, Action clickCallback)
    {
        _title.text = title;
        _button.onClick.AddListener(() => clickCallback());
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
