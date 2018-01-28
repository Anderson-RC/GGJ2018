using UnityEngine;


public class SpriteFacing : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
    }
}