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
        if (!pauseController.paused && player != null) // Optimization on player destroyed
        {
            // If not in car, camera positions based on cursor location on game window
            if (!playerController.enterCar)
            {
                // Ive no idea how this is calculated again but it works:D

                /* Di blok ini, aku mau ngambil Input.mousePosition, tapi yang direturn dari variabel itu adalah
                 * nilai pixel posisi kursor kita pada layar, ini bikin nilai Input.mousePosition ga konsisten antar
                 * layar yang beda-beda resolusinya.
                 * 
                 * Karena itu, aku nyoba ngebagi mousePos dengan lebar layar agar hasilnya menjadi antara 1 dgn 0 atau rasio
                 * mouse dari tengah ke pinggir layar. Input.mousePosition return nilai negatif ketika kursor di 
                 * bawah/kiri layar. Karena nilai 0 berada pada tengah layar, maka input tersebut kita kurangi dari 
                 * setengah tinggi/lebar resolusi layar. Ini memberikan kita nilai input posisi kursor yang konsisten 
                 * antar resolusi-resolusi layar.
                 * 
                 * Coba aja bang, semalaman aku nyoba mecahin kodingan kamera dinamis ini :"(. Aku tau ada cara
                 * lain yang lebih mudah tapi ya bodoamat gtw wkwk.*/
                
                float ratio = Camera.main.aspect;
                float horizontalRatio = 1f;
                float verticalRatio = 1f;

                // Fix bug sensitivitas horizontal lebih besar untuk layar vertical
                if (Screen.width > Screen.height) verticalRatio = ratio;
                else horizontalRatio = ratio;

                float mousePosX = (Input.mousePosition.x) - (Screen.width / 2);
                float mousePosY = (Input.mousePosition.y) - (Screen.height / 2);

                float xPos = mousePosX / Screen.width;
                float yPos = mousePosY / Screen.height;
       
                float playerX = player.transform.position.x;
                float playerY = player.transform.position.z;

                float finalPosX = playerX + (xPos * sensitivity * verticalRatio);
                float finalPosY = playerY + (yPos * sensitivity * horizontalRatio);

                transform.localPosition = new Vector3
                    (finalPosX, camDistance, finalPosY);
            }

            // Biar pas di mobil ga perlu gerak-gerakin mouse buat ngeliat, aku bedain dari yang pas jalan
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
