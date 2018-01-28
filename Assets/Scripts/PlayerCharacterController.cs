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
            HUD.SendMessage("Avoid the Authorities, they shoot on sight!");
            gameController.PlayWound();
            stamina -= 0.21f;
        }

        //goodhat
        if (other.gameObject.layer == 18)
        {
            if (other.GetComponent<GoodHatController>().healthpack > 0)
            {
                HUD.SendMessage("Your follower healed you and confused the police");
            }
            else
            {
                HUD.SendMessage("Your follower confused the police");
            }

            gameController.PlayPrank();
            other.GetComponent<GoodHatController>().CallPolis(gameController);
            stamina += other.GetComponent<GoodHatController>().healthpack;
            other.GetComponent<GoodHatController>().healthpack = 0.0f;
        }
        //badhat
        if (other.gameObject.layer == 19)
        {
            gameController.Play911();
            HUD.SendMessage("That spineless suit called the Cops on you!");
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
            HUD.SendMessage("You Failed, but the message must live on!", 10.0f);
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