using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public List<GameObject> getRoadsInTile(GameObject Tile)
    {

        return null;
    }
    public UnityTile[] getTilesinMap()
    {
        return GetComponents<UnityTile>();
    }

}
