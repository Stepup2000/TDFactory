using UnityEngine;
using UnityEngine.AI;

public class NavMeshEnemyMovement : MonoBehaviour, IMoveable
{
    [SerializeField] private float _speed = 1;

    private NavMeshAgent _navMeshAgent;
    private Transform[] _myPath;
    private int _waypointIndex = 0;

    public void Initialize(Transform[] pPath)
    {
        _myPath = pPath;
        TryGetComponent<NavMeshAgent>(out _navMeshAgent);
        _navMeshAgent.autoBraking = false; // Disable auto braking to ensure continuous movement
        _navMeshAgent.speed = _speed;
        SetCurrentWaypoint(_waypointIndex);
    }

    public void SetCurrentWaypoint(int number)
    {
        _waypointIndex = number;
        if (_waypointIndex < _myPath.Length && _navMeshAgent != null)
        {
            _navMeshAgent.SetDestination(_myPath[_waypointIndex].position);
        }
        else
        {
            ReachedEnd();
        }
    }

    public int GetCurrentWaypoint()
    {
        return _waypointIndex;
    }

    public void GetNextWaypoint()
    {
        _waypointIndex++;
        SetCurrentWaypoint(_waypointIndex);
    }

    private void ReachedEnd()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _myPath[_waypointIndex])
        {
            GetNextWaypoint();
        }
    }
}
