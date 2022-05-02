using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public sealed class AccountRemover : MonoBehaviour
{
    [SerializeField] private Button _removeIdButton;

    private void Awake()
    {
        _removeIdButton.onClick.AddListener(RemoveId);
    }

    private void OnDestroy()
    {
        _removeIdButton.onClick.RemoveAllListeners();
    }

    private void RemoveId()
    {
        if(PlayerPrefs.HasKey(PlayFabSignin.idStorageKey))
            PlayerPrefs.DeleteKey(PlayFabSignin.idStorageKey);
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("Start");
    }
}
