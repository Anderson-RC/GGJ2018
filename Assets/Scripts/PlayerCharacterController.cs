// MoveToClickPoint.cs
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacterController : MonoBehaviour
{
    NavMeshAgent agent;
    public Vector3 destinationEcho;
    public float stamina = 100.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }



    void Update()
    {
        CheckForMovement();
    }


    void CheckForMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                agent.destination = hit.point;
                destinationEcho = agent.destination;
            }
        }
    }
}