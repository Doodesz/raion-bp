using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject player;
    public GameObject camCenterPoint;
    
    private Camera cam;

    public float camDistance = 30f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Agar pergerakan kamera halus, pakai LateUpdate()
    private void LateUpdate()
    {
        transform.position = new Vector3(camCenterPoint.transform.position.x, 
            camDistance, camCenterPoint.transform.position.z);
    }
}
