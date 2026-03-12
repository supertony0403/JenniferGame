using UnityEngine;

public class PoliceOfficer : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointTolerance = 0.5f;
    [SerializeField] private float sightRange = 8f;

    private int _currentWaypoint;
    private Transform _player;
    private int _officerId;
    private static int _nextId = 0;

    private void Awake() => _officerId = _nextId++;

    private void Start()
    {
        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null) _player = playerGo.transform;
    }

    private void Update()
    {
        Patrol();
        CheckPlayerSight();
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        Transform target = waypoints[_currentWaypoint];
        transform.position = Vector3.MoveTowards(
            transform.position, target.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) < waypointTolerance)
            _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
    }

    private void CheckPlayerSight()
    {
        if (_player == null) return;
        float dist = Vector3.Distance(transform.position, _player.position);
        bool inSight = dist <= sightRange;
        PoliceOfficerTracker.Instance?.ReportSight(_officerId, inSight);
        if (inSight) PoliceAttentionSystem.Instance?.AddAttentionFromPatrol(0.1f * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
