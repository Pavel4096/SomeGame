using Photon.Pun;
using UnityEngine;

public sealed class PlayerController : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
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
    private InventoryController _inventoryController;

    private const string _VERTICAL_AXIS = "Vertical";
    private const string _HORIZONTAL_AXIS = "Horizontal";
    private readonly KeyCode _throwGrenadeKey = KeyCode.LeftAlt;
    private const string _grenadeItemId = "Grenade";
    private const string _grenadePrefab = "Grenade";
    private const float _grenadeImpulseMultiplier = 5.0f;
    private const float _grenadeVerticalOffset = 2.0f;

    private void Awake()
    {
        if(_photonView.IsMine)
        {
            _camera.SetActive(true);
            Camera.main.gameObject.SetActive(false);
            _inventoryController = new InventoryController();
        }
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

        bool newIsRunning;
        if(Input.GetAxis(_VERTICAL_AXIS) > 0)
        {
            newIsRunning = true;
        }
        else
            newIsRunning = false;
        
        if(_isRunning != newIsRunning)
        {
            _isRunning = newIsRunning;
            _animator.SetBool("IsRunning", _isRunning);
        }

        float rotationAngle = Input.GetAxis(_HORIZONTAL_AXIS) * _rotationSpeed * Time.deltaTime;
        _rigidbody.MoveRotation(_modelTransform.rotation * Quaternion.AngleAxis(rotationAngle, transform.up));

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
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _hp = (float) info.photonView.InstantiationData[0];
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
