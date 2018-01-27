using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Data;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject policeCarPrefab;
    public GameObject playerPrefab;
    public bool playerExists = false;

    public List<GameObject> tiles = new List<GameObject>();
    public List<GameObject> activePolis = new List<GameObject>();


    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        this.CheckForTileSpawning();
        this.CheckForPlayerSpawn();

    }

    public void CheckForPlayerSpawn()
    {
        //if we don't have a player yet, check for mouse click.  
        if (!playerExists)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;
                    if (objectHit.gameObject.tag == "Building")
                    {
                        this.SpawnPlayerNearBuilding(objectHit);
                    }
                }
            }
        }
    }

    public void SpawnPlayerNearBuilding(Transform building)
    {
        //set player existence to true on spawn    
        GameObject tile = building.parent.gameObject;
        Vector3 closestRoad = ClosestRoadPointToLocationInTile(building.position, tile);
        Instantiate(playerPrefab, closestRoad, Quaternion.identity);
        this.playerExists = true;
        getNumberofClosestPolis(4, building.position);
    }

    public void CheckForTileSpawning()
    {
        List<GameObject> tilesToRemove = new List<GameObject>();
        foreach (GameObject maptile in tiles)
        {
            if (maptile.GetComponent<BoxCollider>() & (maptile.transform.childCount > 0))
            {
                spawnPoliceCar(maptile);
                tilesToRemove.Add(maptile);
            }
        }

        foreach (GameObject maptile in tilesToRemove)
        {
            tiles.Remove(maptile);
        }
    }

    public GameObject[] getRoadsInTile(GameObject tile)
    {
        List<GameObject> roadsList = new List<GameObject>();

        for (int i = 0; i < tile.transform.childCount; i++)
        {
            if (tile.transform.GetChild(i).tag == "Road")
            {
                roadsList.Add(tile.transform.GetChild(i).gameObject);
            }
        }
        return roadsList.ToArray();
    }

    public UnityTile[] getTilesinMap()
    {
        return GetComponents<UnityTile>();
    }

    public Collider[] getCollisionsInTile(GameObject tile)
    {
        BoxCollider tileBox = tile.GetComponent<BoxCollider>();
        Vector3 tileCenter = tile.transform.position;
        Vector3 halfExtents = tileBox.size / 2.0f;
        return Physics.OverlapBox(tileCenter, halfExtents);
    }

    public Vector3 ClosestRoadPointToLocationInTile(Vector3 location, GameObject tile)
    {
        GameObject[] roads = getRoadsInTile(tile);
        List<Vector3> roadPoints = new List<Vector3>();
        float magnitude = float.MaxValue;
        Vector3 closestRoad = new Vector3();
        foreach (GameObject road in roads)
        {
            MeshCollider roadCollider = road.GetComponent<MeshCollider>();
            //Vector3 roadPoint = Physics.ClosestPoint(RandPosInTile, roadCollider, roadCollider.transform.position, Quaternion.identity);
            //Vector3 roadPoint = roadCollider.ClosestPoint(RandPosInTile);
            Vector3 roadPoint = roadCollider.transform.position;
            float distance = Vector3.Distance(location, roadPoint);
            if (distance < magnitude)
            {
                magnitude = distance;
                closestRoad = roadPoint;
            }
        }
        return closestRoad;
    }

    public void spawnPoliceCar(GameObject tile)
    {
        BoxCollider coll = tile.GetComponent<BoxCollider>();
        float halfExtent = coll.size.x / 2.0f;
        Vector3 RandPosInTile = tile.transform.position 
            + new Vector3(Random.Range(-1* halfExtent,halfExtent), 0, Random.Range(-1 * halfExtent,halfExtent));
        Vector3 closestRoad = this.ClosestRoadPointToLocationInTile(RandPosInTile, tile);
        //GameObject[] roads = getRoadsInTile(tile);
       // List<Vector3> roadPoints = new List<Vector3>();
        //float magnitude = float.MaxValue;
        //Vector3 closestRoad = new Vector3();
        //foreach (GameObject road in roads)
        //{
        //    MeshCollider roadCollider = road.GetComponent<MeshCollider>();
        //    //Vector3 roadPoint = Physics.ClosestPoint(RandPosInTile, roadCollider, roadCollider.transform.position, Quaternion.identity);
        //    //Vector3 roadPoint = roadCollider.ClosestPoint(RandPosInTile);
        //    Vector3 roadPoint = roadCollider.transform.position;
        //    float distance = Vector3.Distance(RandPosInTile, roadPoint);
        //    if (distance < magnitude)
        //    {
        //        magnitude = distance;
        //        closestRoad = roadPoint;
        //    }
        //}

        //Spawn a car
        this.activePolis.Add(Instantiate(policeCarPrefab, closestRoad, Quaternion.identity));   
    }

    public List<GameObject> getNumberofClosestPolis(int numberOfPolis, Vector3 requestLocation)
    {
        if (numberOfPolis < 1) { return new List<GameObject>(); }
        int numberToTake = activePolis.Count() > numberOfPolis ? numberOfPolis : activePolis.Count();
        List<GameObject> sortedPolis = this.activePolis.OrderBy(a => Vector3.Distance(requestLocation, a.transform.position)).Take(numberToTake).ToList();
        return sortedPolis;
    }

}
