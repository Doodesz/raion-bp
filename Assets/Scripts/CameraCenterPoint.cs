using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCenterPoint : MonoBehaviour
{
    public float sensitivity = 1.5f;
    public GameObject player;
    public GameObject car;
    public GameObject carCamPoint;

    private EventManager eventManager;
    private GameObject cam;
    private CarBehaviour carBehaviour;
    private PlayerController playerController;
    private float camDistance;
    private bool carRepaired;
    private PauseController pauseController;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        camDistance = cam.GetComponent<CameraBehaviour>().camDistance;
        carBehaviour = car.GetComponent<CarBehaviour>();
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        playerController = player.GetComponent<PlayerController>();
        pauseController = GameObject.Find("Event Manager").GetComponent<PauseController>();
    }

    private void Update()
    {
        if (!pauseController.paused && player != null)
        {
            if (!playerController.enterCar)
            {
                // Ive no idea how this is calculated again but it works:D
                float ratio = Camera.main.aspect;

                float mousePosX = (Input.mousePosition.x) - (Screen.width / 2);
                float mousePosY = (Input.mousePosition.y) - (Screen.height / 2);

                float xPos = mousePosX / Screen.width;
                float yPos = mousePosY / Screen.height;
       
                float playerX = player.transform.position.x;
                float playerY = player.transform.position.z;

                float finalPosX = playerX + (xPos * sensitivity * ratio);
                float finalPosY = playerY + (yPos * sensitivity);

                transform.localPosition = new Vector3
                    (finalPosX, camDistance, finalPosY);
            }

            else if (playerController.enterCar)
            {
                Vector3 camPointPos = new Vector3
                    (player.transform.position.x, camDistance, player.transform.position.z);
                transform.position = camPointPos;
            }
            carRepaired = eventManager.carRepaired;
        }
    }
}
