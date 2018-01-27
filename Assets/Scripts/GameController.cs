﻿using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Data;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject policeCarPrefab;
    public List<GameObject> tiles = new List<GameObject>();

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        List<GameObject> tilesToRemove = new List<GameObject>();
	    foreach (GameObject maptile in tiles)
        {
            if(maptile.GetComponent<BoxCollider>() & (maptile.transform.childCount > 0))
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
        //GameObject[] pieces = tile.transform.GetComponentsInChildren<GameObject>();
        //return pieces.Where(child => child.tag == "Road").ToArray();
        List<GameObject> roadsList = new List<GameObject>();

        //foreach (Transform child in transform)
        //{
        //    if (child.tag == "Road")
        //    {
        //        roadsList.Add(child.gameObject);
        //    }
        //}
        //return roadsList.ToArray();

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

    public void spawnPoliceCar(GameObject tile)
    {
        BoxCollider coll = tile.GetComponent<BoxCollider>();
        Vector3 RandPosInTile = tile.transform.position + new Vector3(coll.size.x / 2.0f, 0, coll.size.z / 2.0f);
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
            float distance = Vector3.Distance(RandPosInTile, roadPoint);
            if (distance < magnitude)
            {
                magnitude = distance;
                closestRoad = roadPoint;
            }
        }

        //Spawn a car
        Instantiate(policeCarPrefab, closestRoad, Quaternion.identity);
        
    }

}