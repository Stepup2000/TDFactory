using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls enemy movement using Unity's NavMesh system.
/// Implements the IMoveable interface to handle pathfinding and waypoint navigation.
/// </summary>
public class NavMeshEnemyMovement : MonoBehaviour, IMoveable
{
    [field: SerializeField] public float speed { get; set; }

    private NavMeshAgent _navMeshAgent;
    private Transform[] _myPath;
    private int _waypointIndex = 0;

    /// <summary>
    /// Initializes the enemy with a path and sets up the NavMeshAgent.
    /// </summary>
    /// <param name="pPath">Array of waypoints for the enemy to follow.</param>
    public void Initialize(Transform[] pPath)
    {
        _myPath = pPath;
        TryGetComponent<NavMeshAgent>(out _navMeshAgent);
        if (_navMeshAgent != null)
        {
            _navMeshAgent.autoBraking = false;
            _navMeshAgent.speed = speed;
            SetCurrentWaypoint(_waypointIndex);
        }
        else
        {
            Debug.LogWarning("NavMeshAgent component not found on the enemy.");
        }
    }

    /// <summary>
    /// Changes the speed based on the give amount.
    /// </summary>
    /// <param name="amount">The amount the speed will change.</param>
    public void AlterSpeed(float amount)
    {
        speed += amount;
        _navMeshAgent.speed = speed;
    }

    /// <summary>
    /// Sets the destination of the NavMeshAgent to the specified waypoint.
    /// </summary>
    /// <param name="number">Index of the waypoint to move to.</param>
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

    /// <summary>
    /// Gets the index of the current waypoint.
    /// </summary>
    /// <returns>Index of the current waypoint.</returns>
    public int GetCurrentWaypoint()
    {
        return _waypointIndex;
    }

    /// <summary>
    /// Moves to the next waypoint in the path.
    /// </summary>
    public void GetNextWaypoint()
    {
        _waypointIndex++;
        SetCurrentWaypoint(_waypointIndex);
    }

    /// <summary>
    /// Called when the enemy reaches the end of the path, destroying the enemy GameObject.
    /// </summary>
    private void ReachedEnd()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Triggered when the enemy collides with a collider.
    /// Moves to the next waypoint if the collider matches the current waypoint.
    /// </summary>
    /// <param name="other">The collider that the enemy has entered.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _myPath[_waypointIndex])
        {
            GetNextWaypoint();
        }
    }
}
