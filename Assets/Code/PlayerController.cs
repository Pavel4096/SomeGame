using Photon.Pun;
using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private GameObject _camera;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    private object[] _data = new object[2];

    private const string _VERTICAL_AXIS = "Vertical";
    private const string _HORIZONTAL_AXIS = "Horizontal";

    private void Awake()
    {
        if(_photonView.IsMine)
        {
            _camera.SetActive(true);
            Camera.main.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(!_photonView.IsMine)
            return;
        
        transform.position += Input.GetAxis(_VERTICAL_AXIS) * _speed * transform.forward * Time.deltaTime;
        float rotationAngle = Input.GetAxis(_HORIZONTAL_AXIS) * _rotationSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(rotationAngle, transform.up);

        _data[0] = transform.position;
        _data[1] = transform.rotation;

        _photonView.RPC("UpdatePosition", RpcTarget.Others, _data);
    }

    [PunRPC]
    private void UpdatePosition(object[] data)
    {
        transform.position = (Vector3) data[0];
        transform.rotation = (Quaternion) data[1];
    }
}
