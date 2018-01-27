using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something Triggered Objective");
        if (other.gameObject.layer == 16)
        {
            Debug.Log("It was the Player!");
            other.gameObject.GetComponent<PlayerCharacterController>().CompleteObjective();
            Destroy(gameObject);
           
        }
    }
}
