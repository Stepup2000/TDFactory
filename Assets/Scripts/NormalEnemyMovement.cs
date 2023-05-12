using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyMovement : MonoBehaviour, IMoveable
{
    [SerializeField] private float _speed = 1;

    private Transform[] _myPath;
    private Transform _target;
    private int _waypointIndex = 0;
    private Vector3 _targetPosition = new Vector3();

    public void Initialize(Transform[] pPath)
    {
        _myPath = pPath;
    }

    // Start is called before the first frame update
    private void Start()
    {
        UpdateTarget();
    }

    public void SetCurrentWaypoint(int number)
    {
        _waypointIndex = number;
        UpdateTarget();
    }

    public int GetCurrentWaypoint()
    {
        return _waypointIndex;
    }

    public void GetNextWaypoint()
    {
        if (_myPath != null && _myPath.Length > 0)
        {
            if (_waypointIndex >= _myPath.Length - 1)
            {
                ReachedEnd();
            }
            else
            {
                _waypointIndex++;
                UpdateTarget();
            }
        }
        else
        {
            Debug.LogError("Path is null or empty.");
        }
    }

    private void UpdateTarget()
    {
        _target = _myPath[_waypointIndex];
        _targetPosition = _target.transform.position;
    }

    public void TryMove()
    {
        Vector3 dir = _targetPosition - transform.position;
        transform.Translate(dir.normalized * _speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, _targetPosition) <= 0.05f)
        {
            GetNextWaypoint();
        }
    }
    private void ReachedEnd()
    {
        Destroy(gameObject);
    }
}
