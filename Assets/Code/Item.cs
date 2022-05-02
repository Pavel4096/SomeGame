using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class Item : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _price;

    public void SetData(string title, string price)
    {
        _title.text = title;
        _price.text = price;
    }
}
