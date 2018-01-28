using UnityEngine;
using UnityEngine.AI;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class GoodHatController : MonoBehaviour
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
        CONVERTING,
        DISTRACTING,
        STOPPED,
        
    }
    public State state;
    public float distractionLength = 5.0f;

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
        if (this.state == State.CONVERTING) { WalkingUpdate(); }
        if (this.state == State.DISTRACTING) { DistractingUpdate(); }
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
    
    private void DistractingUpdate()
    {
        if (_waitTime < 0f)
        {
            targetEntity = null;
            SetWalking();
        }
        else if ((transform.position - _agent.destination).sqrMagnitude < 1 )
        {
            targetEntity.GetComponent<PolisCarController>().DistractCar(distractionLength);
            targetEntity = null;
            StopGoodHat(distractionLength);
        }
        else
        {
            _agent.destination = targetEntity.transform.position;
            _waitTime -= Time.deltaTime;
        }
    }
    public void StopGoodHat(float stoppedTime = 5.0f)
    {
        this.state = State.STOPPED;
        this._stoppedTimer = stoppedTime;
    }
    private void StoppedUpdate()
    {
        _stoppedTimer -= Time.deltaTime;
        if (_stoppedTimer < 0f) { this.SetWalking(); }
    }
    public void SetWalking()
    {
        this.state = State.WALKING;
    }

    public void CallPolis(GameController gameController)
    {
        if (this.state == State.WALKING)
        {
            List<GameObject> nearbyPolis = gameController.getNumberofClosestPolis(10, this.transform.position);
            Vector3 loc = (Random.insideUnitCircle.normalized * Random.Range(300.0f, 500.0f)).ToVector3xz() + this.transform.position;
            foreach (GameObject polis in nearbyPolis)
            {
                polis.GetComponent<PolisCarController>().TipOffPolis(loc);
            }
            StopGoodHat();
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