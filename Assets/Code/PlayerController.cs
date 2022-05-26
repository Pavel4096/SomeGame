using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public sealed class PlayerController : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnEventCallback
{
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private GameObject _camera;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    private bool _isRunning;
    private float _hp;
    private float _damage;
    private InventoryController _inventoryController;
    private float _lastShootTime;
    private PlayerInformation _playerInformation;
    private int _currentXP;

    private const string _VERTICAL_AXIS = "Vertical";
    private const string _HORIZONTAL_AXIS = "Horizontal";
    private const float _minInputToRun = 0.1f;
    private const float _minInputToRotate = 0.1f;
    private readonly KeyCode _throwGrenadeKey = KeyCode.LeftAlt;
    private readonly KeyCode _fireKey = KeyCode.Space;
    private const string _grenadeItemId = "Grenade";
    private const string _grenadePrefab = "Grenade";
    private const float _grenadeImpulseMultiplier = 5.0f;
    private const float _grenadeVerticalOffset = 2.0f;
    private const float _timeBetweenShoots = 0.1f;
    private const byte _shootCode = 1;
    private const int _xpToAdd = 500;

#if UNITY_ANDROID
    private Vector2 _centerPosition;
#endif

    private void Awake()
    {
        if(_photonView.IsMine)
        {
            _camera.SetActive(true);
            Camera.main.gameObject.SetActive(false);
            _inventoryController = new InventoryController();
        }

        PlayerData playerData = Object.FindObjectOfType<PlayerData>();
        _playerInformation = playerData?.PlayerInformation;

        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest {
            CharacterId = _playerInformation.CharacterId
        }, ProcessAvailableStatistics, LogError);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _hp -= 10.0f;
        _photonView.RPC("UpdateHP", RpcTarget.Others, _hp);
    }

    private void Update()
    {
        if(!_photonView.IsMine)
            return;
        
        Vector2 currentInput = new Vector2(0, 0);
        bool shoot = false;

#if UNITY_ANDROID
        if(Input.touchCount > 0)
        {
            Touch currentTouch = Input.GetTouch(0);
            if(currentTouch.phase == TouchPhase.Began)
                _centerPosition = currentTouch.position;
            currentInput = (currentTouch.position - _centerPosition)/200.0f;

            if(Input.touchCount > 1)
            {
                Touch fireTouch = Input.GetTouch(1);
                if(fireTouch.phase == TouchPhase.Began)
                    shoot = true;
            }
        }
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
            currentInput = new Vector2(Input.GetAxis(_HORIZONTAL_AXIS),  Input.GetAxis(_VERTICAL_AXIS));
            shoot = Input.GetKeyDown(_fireKey);
#endif

            if((currentInput.x > -_minInputToRotate) && (currentInput.x < _minInputToRotate))
                currentInput.x = 0.0f;
            if((currentInput.y > -_minInputToRun) && (currentInput.y < _minInputToRun))
                currentInput.y = 0.0f;

        bool newIsRunning = currentInput.y > 0;
        
        if(_isRunning != newIsRunning)
        {
            _isRunning = newIsRunning;
            _animator.SetBool("IsRunning", _isRunning);
        }

        float rotationAngle = currentInput.x * _rotationSpeed * Time.deltaTime;
        _rigidbody.MoveRotation(_modelTransform.rotation * Quaternion.AngleAxis(rotationAngle, transform.up));
        _modelTransform.rotation = _modelTransform.rotation * Quaternion.AngleAxis(rotationAngle, transform.up);

        transform.position = _modelTransform.position;
        transform.rotation = _modelTransform.rotation;
        _modelTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _modelTransform.localRotation = Quaternion.identity;

        if(Input.GetKeyDown(_throwGrenadeKey))
        {
            if(_inventoryController.UseInventoryItem(_grenadeItemId))
            {
                Vector3 position = transform.position + transform.up * _grenadeVerticalOffset;
                GameObject grenade = PhotonNetwork.Instantiate(_grenadePrefab, position, transform.rotation);
                Vector3 impulse = (transform.forward + transform.up).normalized * _grenadeImpulseMultiplier;
                grenade.GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
            }
        }

        if(shoot && (Time.time - _lastShootTime) > _timeBetweenShoots)
        {
            Shoot();
        }
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData eventData)
    {
        if(eventData.Code == _shootCode)
        {
            object[] data = (object[]) eventData.CustomData;

            Debug.Log($"{data.GetType()}");
            Debug.Log($"{data.Length}");
            int viewID = (int) data[0];
            float damage = (float) data[1];

            if(_photonView.ViewID == viewID && _photonView.IsMine)
            {
                _hp -= damage;
                UpdateHP(_hp);
                if(_hp <= 0)
                    PhotonNetwork.LeaveRoom();
            }
        }
    }

    private void Shoot()
    {
        if(Physics.Raycast(transform.position + transform.up * 0.25f, transform.forward, out RaycastHit hit))
        {
            PhotonView photonView = hit.transform.GetComponent<PhotonView>();

            if(photonView != null && !photonView.IsMine)
            {
                RaiseEventOptions raiseOptions = new RaiseEventOptions {
                    Receivers = ReceiverGroup.All
                };
                
                object[] data = new object[2];
                data[0] = photonView.ViewID;
                data[1] = _damage;

                PhotonNetwork.RaiseEvent(_shootCode, data, raiseOptions, new SendOptions {
                    Reliability = true
                });

                UpdateStatistics();
            }
            _lastShootTime = Time.time;
        }
    }

    private void UpdateStatistics()
    {
        _currentXP += _xpToAdd;
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest {
            CharacterId = _playerInformation.CharacterId,
            CharacterStatistics = new Dictionary<string, int> {
                ["XP"] = _currentXP
            }
        }, null, LogError);
    }

    private void ProcessAvailableStatistics(GetCharacterStatisticsResult result)
    {
        int availableXP = 0;
        if(result.CharacterStatistics.ContainsKey("XP"))
            availableXP = result.CharacterStatistics["XP"];

        _currentXP += availableXP;

        Debug.Log($"CurrentXP: {_currentXP}");
    }

    private void LogError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _hp = (float) info.photonView.InstantiationData[0];
        _damage = (float) info.photonView.InstantiationData[1];

        if(_photonView.IsMine)
            Debug.Log($"HP: {_hp} Damage: {_damage}");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(_isRunning);
        }
        else if(stream.IsReading)
        {
            transform.position = (Vector3) stream.ReceiveNext();
            transform.rotation = (Quaternion) stream.ReceiveNext();
            _modelTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            _modelTransform.localRotation = Quaternion.identity;
            bool newIsRunning = (bool) stream.ReceiveNext();

            if(_isRunning != newIsRunning)
            {
                _isRunning = newIsRunning;
                _animator.SetBool("IsRunning", _isRunning);
            }
        }
    }

    [PunRPC]
    private void UpdateHP(float hp)
    {
        _hp = hp;
        Debug.Log($"New HP: {_hp}");
    }
}
