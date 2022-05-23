using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public sealed class CharactersWindow : MonoBehaviour
{
    [SerializeField] private Transform _charactersRoot;
    [SerializeField] private ShopWindow _shopWindow;
    [SerializeField] private NewCharacterWindow _newCharacterWindow;
    [SerializeField] private CharacterItem[] _characters;
    private List<CharacterResult> _userCharacters;
    private List<PlayerInformation> _playerInformations;
    private PlayerData _playerData;

    private void Start()
    {
        _shopWindow.CatalogLoaded += UpdateCharactersList;
        _newCharacterWindow.NewCharacterAdded += UpdateCharactersList;

        for(var i = 0; i < _characters.Length; i++)
        {
            _characters[i].AddNewCharacter += _newCharacterWindow.OpenWindow;
            _characters[i].CharacterSelected += ProcessCharacterSelection;
        }

        _newCharacterWindow.CloseWindow();
    }

    private void OnDestroy()
    {
        _shopWindow.CatalogLoaded -= UpdateCharactersList;
        _newCharacterWindow.NewCharacterAdded -= UpdateCharactersList;

        for(var i = 0; i < _characters.Length; i++)
        {
            _characters[i].AddNewCharacter -= _newCharacterWindow.OpenWindow;
            _characters[i].CharacterSelected -= ProcessCharacterSelection;
        }
    }

    private void UpdateCharactersList()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(), ProcessCharacters, LogError);
    }

    private void ProcessCharacters(ListUsersCharactersResult result)
    {
        _userCharacters = result.Characters;
        _playerInformations = new List<PlayerInformation>();

        for(var i = 0; i < _characters.Length; i++)
        {
            _characters[i].ClearCharacter();
        }

        for(var i = 0; i < result.Characters.Count; i++)
        {
            if(i < _characters.Length)
            {
                PlayerInformation playerInformation = JsonUtility.FromJson<PlayerInformation>(_shopWindow.CatalogItems[result.Characters[i].CharacterType].CustomData);
                _playerInformations.Add(playerInformation);
                _characters[i].SetCharacter(result.Characters[i].CharacterName, playerInformation.MaxHP, playerInformation.Damage, i);
            }
        }
    }

    private void ProcessCharacterSelection(int id)
    {
        Debug.Log($"Selected character: {_userCharacters[id].CharacterName} - MaxHP: '{_playerInformations[id].MaxHP}' Damage: {_playerInformations[id].Damage}");
    }

    private void LogError(PlayFabError error)
    {}
}
