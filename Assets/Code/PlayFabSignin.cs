using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using Object = UnityEngine.Object;

public sealed class PlayFabSignin : MonoBehaviour
{
    [Header("Account creation")]
    [SerializeField] private GameObject _createAccountWindow;
    [SerializeField] private TMP_InputField _createUserName;
    [SerializeField] private TMP_InputField _createUserEmail;
    [SerializeField] private TMP_InputField _createUserPassword;
    [SerializeField] private Button _createAccountButton;
    [SerializeField] private Button _useExistingAccountButton;
    [SerializeField] private Button _signupWithoutAccountCreation;

    [Header("Sign in")]
    [SerializeField] private GameObject _signinWindow;
    [SerializeField] private TMP_InputField _signinUserName;
    [SerializeField] private TMP_InputField _signinUserPassword;
    [SerializeField] private Button _signinButton;
    [SerializeField] private Button _createNewAccountButton;
    [Space(50)]
    [SerializeField] private GameObject _errorTextWindow;
    private TMP_Text _errorText;

    public static readonly string idStorageKey = "PlayFabUserGUID";
    private string _userGUID;
    private bool _needsSaveGUID;
    private IResultWaiter _resultWaiter;

    private void Awake()
    {
        _useExistingAccountButton.onClick.AddListener(SwitchWindows);
        _createNewAccountButton.onClick.AddListener(SwitchWindows);
        _createAccountButton.onClick.AddListener(CreateAccount);
        _signupWithoutAccountCreation.onClick.AddListener(CreateAccountUsingID);
        _signinButton.onClick.AddListener(Signin);
        _errorText = _errorTextWindow.GetComponentInChildren<TMP_Text>();
        ClearErrorText();
    }

    private void Start()
    {
        _resultWaiter = Object.FindObjectOfType<ResultWaiter>();

        if(PlayerPrefs.HasKey(PlayFabSignin.idStorageKey))
        {
            string userId = PlayerPrefs.GetString(PlayFabSignin.idStorageKey);
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest {
                CustomId = userId
            }, LoginSucceeded, RequestFailed);
            _resultWaiter.ShowWaiter();
        }
    }

    private void OnDestroy()
    {
        _useExistingAccountButton.onClick.RemoveAllListeners();
        _createNewAccountButton.onClick.RemoveAllListeners();
        _createAccountButton.onClick.RemoveAllListeners();
        _signupWithoutAccountCreation.onClick.RemoveAllListeners();
        _signinButton.onClick.RemoveAllListeners();
    }

    private void CreateAccount()
    {
        string userName = _createUserName.text;
        string userEmail = _createUserEmail.text;
        string userPassword = _createUserPassword.text;

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest {
            Username = userName,
            Email = userEmail,
            Password = userPassword,
            RequireBothUsernameAndEmail = true
        }, AccountCreationSucceeded, RequestFailed);
        ClearErrorText();
        _resultWaiter.ShowWaiter();
    }

    private void CreateAccountUsingID()
    {
        _userGUID = Guid.NewGuid().ToString();
        _needsSaveGUID = true;
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest {
            CustomId = _userGUID,
            CreateAccount = true
        }, LoginSucceeded, RequestFailed);
        _resultWaiter.ShowWaiter();
    }

    private void Signin()
    {
        string userName = _signinUserName.text;
        string userPassword = _signinUserPassword.text;

        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest {
            Username = userName,
            Password = userPassword
        }, LoginSucceeded, RequestFailed);
        ClearErrorText();
        _resultWaiter.ShowWaiter();
    }

    private void SwitchWindows()
    {
        _createAccountWindow.SetActive(!_createAccountWindow.activeSelf);
        _signinWindow.SetActive(!_signinWindow.activeSelf);
        ClearErrorText();
    }

    private void AccountCreationSucceeded(RegisterPlayFabUserResult result)
    {
        _resultWaiter.HideWaiter();
        Debug.Log("Account creation succeeded");
        LoadNextScene();
    }

    private void LoginSucceeded(LoginResult result)
    {
        _resultWaiter.HideWaiter();
        Debug.Log("Login succeeded");
        if(_needsSaveGUID)
            PlayerPrefs.SetString(PlayFabSignin.idStorageKey, _userGUID);
        LoadNextScene();
    }

    private void RequestFailed(PlayFabError error)
    {
        _resultWaiter.HideWaiter();
        Debug.Log($"{error.GenerateErrorReport()}");
        _errorText.text = "Cann't create account or sign in.";
        _errorTextWindow.SetActive(true);
    }

    private void LoadNextScene()
    {
        GameObject playerData = new GameObject("PlayerData", typeof(PlayerData));
        SceneManager.LoadScene("Game");
    }

    private void ClearErrorText()
    {
        _errorText.text = "";
        _errorTextWindow.SetActive(false);
    }
}
