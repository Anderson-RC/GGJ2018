﻿// MoveToClickPoint.cs
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacterController : MonoBehaviour
{
    NavMeshAgent agent;
    public Vector3 destinationEcho;
    public float stamina = 100.0f;
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

    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Collisde");
    //    if (other.gameObject.layer == 15)
    //    {
    //        Debug.Log("Spotted");
    //    }
    //}

    void OnTriggerStay(Collider other)
    {
       if (other.gameObject.layer == 15)
       {
            stamina -= 0.5f;

       }

    }


    void CheckIfSpotted()
    {
        //CollisionDetection with Vision Cones
        //One layer for all vision, then use tags or owner types to decide results

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
                destinationEcho = agent.destination;
            }
        }
    }
}