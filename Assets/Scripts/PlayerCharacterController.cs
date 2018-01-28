// MoveToClickPoint.cs
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacterController : MonoBehaviour
{
    NavMeshAgent agent;
    public float stamina = 1.0f;
    public GameObject objective;
    public GameController gameController;
    public UIController HUD;

    private int objectivesCompleted;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        objectivesCompleted = 0;
    }



    void Update()
    {
        CheckForMovement();
        CheckIfSpotted();
        CheckIfDead();
        HUD.SetHealthSlider(this.stamina);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collisde");
        Debug.Log(other.gameObject);
        //polis
        if (other.gameObject.layer == 15)
        {
            Debug.Log("Spotted");
            stamina -= 0.2f;
        }

        //goodhat
        if (other.gameObject.layer == 18)
        {
            other.GetComponent<GoodHatController>().CallPolis(gameController);
            stamina += other.GetComponent<GoodHatController>().healthpack;
            other.GetComponent<GoodHatController>().healthpack = 0.0f;
        }
        //badhat
        if (other.gameObject.layer == 19)
        {
            Debug.Log("attempting 999 call");
            other.GetComponent<BadHatController>().CallPolis(gameController);
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
        this.objectivesCompleted++;
        HUD.SetObjectiveCounter(this.objectivesCompleted);
        
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