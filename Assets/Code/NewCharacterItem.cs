using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class NewCharacterItem : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _name;

    public void SetData(string name, Action clickCallback)
    {
        _name.text = name;
        _button.onClick.AddListener(() => clickCallback?.Invoke());
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}
