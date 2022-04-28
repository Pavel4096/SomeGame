using UnityEngine;

public sealed class ResultWaiter : MonoBehaviour, IResultWaiter
{
    [SerializeField] private GameObject[] _items;
    [SerializeField] private float _timeBetweenUpdates;

    private bool _isEnabled;
    private int _currentIndex;
    private int _previousIndex;
    private float _lastUpdateTime;

    private void Awake()
    {
        for(var i = 0; i < _items.Length; i++)
        {
            _items[i].SetActive(false);
        }
    }

    private void Update()
    {
        if(!_isEnabled)
            return;
        
        if( (Time.time - _lastUpdateTime) < _timeBetweenUpdates )
            return;
        
        _items[_previousIndex].SetActive(false);
        _items[_currentIndex].SetActive(true);
        _previousIndex = _currentIndex;
        _currentIndex++;
        if(_currentIndex > _items.Length - 1)
            _currentIndex = 0;
        
        _lastUpdateTime = Time.time;
    }

    public void ShowWaiter()
    {
        _isEnabled = true;
    }

    public void HideWaiter()
    {
        _isEnabled = false;
        _items[_previousIndex].SetActive(false);
        _currentIndex = 0;
        _previousIndex = 0;
        _lastUpdateTime = 0;
    }
}
