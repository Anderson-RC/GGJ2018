using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Data;
using System.Linq;
using UnityEngine;
using Mapbox.Unity.Utilities;
using UnityEngine.AI;

public class GameController : MonoBehaviour {

    public GameObject policeCarPrefab;
    public GameObject playerPrefab;
    public GameObject objectivePrefab;
    public GameObject goodHatPrefab;
    public GameObject badHatPrefab;
    public UIController HUD;
    public AudioClip call911;
    public AudioClip prankCall;
    public AudioClip wound;

    public bool playerExists = false;
    public GameObject objective;
    public PlayerCharacterController charControl = null;

    public List<GameObject> tiles = new List<GameObject>();
    public List<GameObject> activePolis = new List<GameObject>();
    public List<GameObject> goodHats = new List<GameObject>();
    public List<GameObject> badHats = new List<GameObject>();

    public AudioSource soundEffects;


    // Use this for initialization
    void Start () {


    }

    // Update is called once per frame
    void Update () {

        this.CheckForTileSpawning();
        this.CheckForPlayerSpawn();

    }

    public void PlayPrank()
    {
        soundEffects.PlayOneShot(prankCall);
    }

    public void Play911()
    {
        soundEffects.PlayOneShot(call911);
    }

    public void PlayWound()
    {
        soundEffects.PlayOneShot(wound);
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
            else if (!HUD.message.enabled)
            {
                HUD.SendMessage("Click a building to start");
            }
        }
    }

 

    public void CreateObjective(GameObject player)
    {
        //find a point on a road at least 300m away from player
        RaycastHit hit;
        Vector3 loc = (Random.insideUnitCircle.normalized * Random.Range(300.0f, 500.0f)).ToVector3xz() + player.transform.position;
        // Gets a vector that points from the objective position to the player.
        var heading = player.transform.position - loc;
        if(Physics.Raycast(loc,heading,out hit,500.0f,17)) {
            SpawnObjectiveNearBuilding(hit.transform);
        }
        else
        {
            CreateObjective(player);
        }

        //objective = (Instantiate(objectivePrefab, loc, Quaternion.identity));
        //player.GetComponent<PlayerCharacterController>().objective = objective;
        //create an object to represent objective once we can see it

    }
    public void SpawnObjectiveNearBuilding(Transform building)
    {
        //set player existence to true on spawn    
        
        GameObject tile = building.parent.gameObject;
        Vector3 closestRoad = ClosestRoadPointToLocationInTile(building.position, tile);
        // Check if closestRoad is reachable
        if (ValidPathExists(closestRoad))
        {
            this.objective = (Instantiate(this.objectivePrefab, closestRoad, Quaternion.identity));
            this.charControl.objective = objective;
            
        }
        else
        {
            CreateObjective(this.charControl.gameObject);
        }

        //NavMeshAgent agent = this.charControl.gameObject.GetComponent<NavMeshAgent>();
        //NavMeshPath path = new NavMeshPath();
        //agent.CalculatePath(closestRoad, path);
        //if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
        //{
        //    CreateObjective(this.charControl.gameObject);
        //}
        //else
        //{
        //    //if so then create objective
        //    this.objective = (Instantiate(this.objectivePrefab, closestRoad, Quaternion.identity));
        //    this.charControl.objective = objective;
        //}
     
    }

    public bool ValidPathExists(Vector3 targetLocation)
    {

        NavMeshAgent agent = this.charControl.gameObject.GetComponent<NavMeshAgent>();
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetLocation, path);
        if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
            return false;
        else
            return true;
    }

    public void SpawnPlayerNearBuilding(Transform building)
    {
        //set player existence to true on spawn    
        GameObject tile = building.parent.gameObject;
        Vector3 closestRoad = ClosestRoadPointToLocationInTile(building.position, tile);
        GameObject player = (Instantiate(playerPrefab, closestRoad, Quaternion.identity));
        player.GetComponent<PlayerCharacterController>().HUD = this.HUD;
        player.GetComponent<PlayerCharacterController>().gameController = this;
        this.charControl = player.GetComponent<PlayerCharacterController>();
        CreateObjective(player);
        this.playerExists = true;
        HUD.SendMessage("Follow the Arrow to your objective", 1000.0f);
        HUD.SetObjectiveCounter(0);

        //getNumberofClosestPolis(4, building.position);
    }

    public void CheckForTileSpawning()
    {
        List<GameObject> tilesToRemove = new List<GameObject>();
        foreach (GameObject maptile in tiles)
        {
            if (maptile.GetComponent<BoxCollider>() & (maptile.transform.childCount > 0))
            {
                spawnPoliceCar(maptile);
                
                //if there are any paths, add people
                for (int i = 0; i < maptile.transform.childCount; i++)
                {
                    if (maptile.transform.GetChild(i).tag == "Path")
                    {
                        spawnPeople(maptile.transform.GetChild(i).gameObject);
                    }
                }
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

    public void spawnPeople(GameObject path)
    {
        if (this.goodHats.Count() < this.badHats.Count())
        {
            this.goodHats.Add(Instantiate(goodHatPrefab, path.transform.position, Quaternion.identity));
        }
        else
        {
            this.badHats.Add(Instantiate(badHatPrefab, path.transform.position, Quaternion.identity));
        }
        
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

        //Spawn a car if path valid, otherwise try again
       
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
