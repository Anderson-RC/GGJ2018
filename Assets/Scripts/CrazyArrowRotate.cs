using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyArrowRotate : MonoBehaviour {

    public GameController gameController;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameController.playerExists || gameController.objective == null)
        {
            this.transform.eulerAngles = new Vector3(90, 0, 0);
            return;
        }
        Vector3 rotvec = (gameController.objective.transform.position - gameController.charControl.transform.position);
        float rotangle = Mathf.Atan2(rotvec.x, rotvec.z) * (180/Mathf.PI);
        this.transform.eulerAngles = new Vector3(90, rotangle, 0);

	}
}
