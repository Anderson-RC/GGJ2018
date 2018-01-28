using UnityEngine;
using UnityEngine.AI;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class BadHatController : MonoBehaviour
{
    [SerializeField]
    Vector2 _timeRange;

    [SerializeField]
    Vector2 _distanceRange;

    public GameObject viewCone;
    public GameObject targetEntity;
    public enum State
    {
        WALKING,
        FLEEING,
        STOPPED,
    }
    public State state;
 

    NavMeshAgent _agent;

    float _elapsedTime;
    float _stoppedTimer;

    float _waitTime;
    bool _isFocused;


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetDestination();
    }

    void Update()
    {
        if (this.state == State.WALKING) { WalkingUpdate(); }
        if (this.state == State.FLEEING) { FleeingUpdate(); }
        if (this.state == State.STOPPED) { StoppedUpdate(); }

    }
    private void WalkingUpdate()
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

    public void StopBadHat(float stoppedTime = 10.0f)
    {
        this.state = State.STOPPED;
        this._stoppedTimer = stoppedTime;
    }
    private void StoppedUpdate()
    {
        _stoppedTimer -= Time.deltaTime;
        if (_stoppedTimer < 0f) {
            this.transform.Find("ActionSprite").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.SetWalking(); }
    }
    public void SetWalking()
    {
        this.state = State.WALKING;
    }

    private void FleeingUpdate()
    {

    }

    public void CallPolis(GameController gameController)
    {
        if (this.state == State.WALKING)
        {
            this.transform.Find("ActionSprite").gameObject.GetComponent<SpriteRenderer>().enabled = true;
            List<GameObject> nearbyPolis = gameController.getNumberofClosestPolis(10, this.transform.position);
            foreach (GameObject polis in nearbyPolis)
            {
                polis.GetComponent<PolisCarController>().TipOffPolis(this.transform.position);
            }
            StopBadHat();
        }

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