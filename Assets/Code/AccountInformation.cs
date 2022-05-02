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

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), InformationLoaded, InformationLoadFail);
        _resultWaiter.ShowWaiter();
    }

    private void InformationLoaded(GetAccountInfoResult result)
    {
        _resultWaiter.HideWaiter();
        ShowInformation(result.AccountInfo);
    }

    private void InformationLoadFail(PlayFabError error)
    {
        _resultWaiter.HideWaiter();
        Debug.Log($"{error.GenerateErrorReport()}");
    }

    private void ShowInformation(UserAccountInfo info)
    {
        _informationText.text = $"User: {info.Username}";
        _informationText.text += $"\nCreated: {info.Created}";
        _informationText.text += $"\nLast login: {info.TitleInfo.LastLogin}";
    }
}
