using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class CharacterItem : MonoBehaviour
{
    public event Action<int> CharacterSelected;
    public event Action AddNewCharacter;

    [SerializeField] private Button _characterButton;
    [SerializeField] private Button _addNewButton;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _maxHP;
    [SerializeField] private TMP_Text _damage;

    private int _id;

    private void Start()
    {
        _characterButton.onClick.AddListener(() => CharacterSelected?.Invoke(_id));
        _addNewButton.onClick.AddListener(() => AddNewCharacter?.Invoke());
    }

    private void OnDestroy()
    {
        _characterButton.onClick.RemoveAllListeners();
        _addNewButton.onClick.RemoveAllListeners();
    }

    public void ClearCharacter()
    {
        _addNewButton.gameObject.SetActive(true);
        _characterButton.gameObject.SetActive(false);
        _name.text = "";
    }

    public void SetCharacter(string name, float maxHP, float damage, int id)
    {
        _addNewButton.gameObject.SetActive(false);
        _characterButton.gameObject.SetActive(true);
        _name.text = name;
        _maxHP.text = $"{maxHP}";
        _damage.text = $"{damage}";
    }
}
