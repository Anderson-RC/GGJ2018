// MoveToClickPoint.cs
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacterController : MonoBehaviour
{
    NavMeshAgent agent;
    public float stamina = 100.0f;
    public GameObject objective;
    public GameController gameController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }



    void Update()
    {
        CheckForMovement();
        CheckIfSpotted();
        CheckIfDead();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collisde");
        Debug.Log(other.gameObject);
        //polis
        if (other.gameObject.layer == 15)
        {
            Debug.Log("Spotted");
        }

        //goodhat
        if (other.gameObject.layer == 18)
        {
            other.GetComponent<GoodHatController>().CallPolis(gameController);
        }
        //badhat
        if (other.gameObject.layer == 19)
        {
            Debug.Log("attempting 999 call");
            other.GetComponent<GoodHatController>().CallPolis(gameController);
        }
    }

    void OnTriggerStay(Collider other)
    {
       if (other.gameObject.layer == 15)
       {
            Debug.Log("staying");
            stamina -= 0.5f;

       }

    }


    void CheckIfSpotted()
    {
        //CollisionDetection with Vision Cones
        //One layer for all vision, then use tags or owner types to decide results

    }

    public void CompleteObjective()
    {
        this.objective = null;
        this.gameController.objective = null;
        this.gameController.CreateObjective(this.gameObject);
    }

    void CheckIfDead()
    {
        if (stamina < 0.0f)
        {
            //Player has failed.  
            //spawn Bad Hat           
            //gc.SpawnBadHat();
            //notify player - 
            //gc.NotifyNextOfKin()
            gameController.playerExists = false;
            Destroy(gameObject);
        }

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
               
            }
        }
    }
}