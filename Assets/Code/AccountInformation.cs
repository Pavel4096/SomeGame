using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

public class AccountInformation : MonoBehaviour
{
    [SerializeField] private TMP_Text _informationText;

    private IResultWaiter _resultWaiter;

    private void Start()
    {
        _resultWaiter = Object.FindObjectOfType<ResultWaiter>();

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), ProfileLoaded, ProfileLoadFail);
        _resultWaiter.ShowWaiter();
    }

    private void ProfileLoaded(GetPlayerProfileResult result)
    {
        _resultWaiter.HideWaiter();
        ShowInformation(result.PlayerProfile);
    }

    private void ProfileLoadFail(PlayFabError error)
    {
        _resultWaiter.HideWaiter();
        Debug.Log($"{error.GenerateErrorReport()}");
    }

    private void ShowInformation(PlayerProfileModel profile)
    {
        _informationText.text = $"Created: {profile.Created}";
        _informationText.text += $"\nLast login: {profile.LastLogin}";
    }
}
