using UnityEngine;
using UnityEngine.AI;
using Mapbox.Unity.Utilities;

[RequireComponent(typeof(NavMeshAgent))]
public class PolisCarController : MonoBehaviour
{
    [SerializeField]
    Vector2 _timeRange;

    [SerializeField]
    Vector2 _distanceRange;

    public GameObject viewCone;
    public enum State
    {
        PATROLLING,
        TIPPEDOFF,
        DISTRACTED,
    }
    public State state;

    NavMeshAgent _agent;

    float _elapsedTime;
    float _waitTime;
    bool _isFocused;

    float _distractedTimer;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetDestination();
        this.SetPatrolling();
    }

    void Update()
    {
        if (this.state == State.PATROLLING) { PatrollingUpdate(); }
        if (this.state == State.TIPPEDOFF) { TippedOffUpdate(); }
        if (this.state == State.DISTRACTED) { DistractedUpdate(); }

    }
    private void PatrollingUpdate()
    {
        Debug.DrawLine(transform.position, _agent.destination, Color.red);
        if (_elapsedTime >= _waitTime || (transform.position - _agent.destination).sqrMagnitude < 1)
        {
            _elapsedTime = 0f;
            _waitTime = Random.Range(_timeRange.x, _timeRange.y);
            SetDestination();
        }
        _elapsedTime += Time.deltaTime;
    }
    private void DistractedUpdate()
    {
        _distractedTimer -= Time.deltaTime;
        if (_distractedTimer < 0f) { this.SetPatrolling(); }
    }
    private void TippedOffUpdate()
    {
        if ((transform.position - _agent.destination).sqrMagnitude < 1 || _waitTime < 0f)
        {
            SetPatrolling();
        }
        _waitTime -= Time.deltaTime;
    }
    public void DistractCar(float distractedTime = 5.0f)
    {
        this.state = State.DISTRACTED;
        this._distractedTimer = distractedTime;
    }
    public void SetPatrolling()
    {
        this.state = State.PATROLLING;
    }
    public void TipOffPolis(Vector3 lastSeen)
    {
        this.state = State.TIPPEDOFF;
        _agent.destination = lastSeen;
        _waitTime =  _timeRange.y;

    }

    void SetDestination()
    {
        var target = (Random.insideUnitCircle * Random.Range(_distanceRange.x, _distanceRange.y)).ToVector3xz() + transform.position;
        if (_agent.isOnNavMesh)
        {
            _agent.destination = target;
        }
    }
}