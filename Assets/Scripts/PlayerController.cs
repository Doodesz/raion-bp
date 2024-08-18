using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float initMoveSpeed = 5f;
    public float initFireRateCD = 0.3f;
    public int hp = 3;
    public bool canInteract = false;
    public bool enterCar = false;
    public bool isDead = false;
    public bool holdingRepairTool = false;

    public GameObject carExitPoint;
    public GameObject ground;
    public GameObject bullet;
    public GameObject bulletSpawnPos;
    public GameObject eventManagerObject;
    public GameObject car;
    public GameObject sliderObject;
    public CarBehaviour carBehaviour;
    public CameraBehaviour cameraBehaviour;
    public GameObject shootAudioObject;
    public GameObject repairIcon;
    public GameObject shootParticleObject;

    public float moveSpeed = 5f;
    private float fireRateCD;
    private bool isReceivingDMG = false;
    private float interactCD = 0f;

    private Interactable currentInteractable;
    private Material color;
    private Color initColor;
    private Camera mainCam;
    private EventManager eventManager;
    private Animator animator;
    private PauseController pauseController;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        fireRateCD = initFireRateCD;
        cameraBehaviour = GameObject.Find("Main Camera").GetComponent<CameraBehaviour>();
        initColor = GetComponent<Renderer>().material.color;
        color = GetComponent<Renderer>().material;
        currentInteractable = eventManagerObject.GetComponent<EventManager>().currentInteractable;
        carBehaviour = car.GetComponent<CarBehaviour>();
        eventManager = eventManagerObject.GetComponent<EventManager>();
        animator = GetComponent<Animator>();
        pauseController = eventManager.GetComponent<PauseController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseController.paused)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            if (eventManager.gameEnded)
            {
                horizontalAxis = 0; verticalAxis = 0;
            }

            // Player Movements
            if (!enterCar)
            {
                // Move horizontally
                transform.Translate(Vector3.right * moveSpeed * horizontalAxis * Time.deltaTime, Space.World);
                // Move vertically
                transform.Translate(Vector3.forward * moveSpeed * verticalAxis * Time.deltaTime, Space.World);
            }

            // Teleport player under car when entered
            else if (enterCar)
            {
                transform.position = car.transform.position + new Vector3(0f, -5f, 0f);
            }

            // Anim
            if (verticalAxis > 0 || horizontalAxis > 0 || verticalAxis < 0 || horizontalAxis < 0)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }

            // Sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = initMoveSpeed * 1.5f;
                animator.SetBool("isSprinting", true);
            }
            else
            {
                moveSpeed = initMoveSpeed;
                animator.SetBool("isSprinting", false);
            }

            // Shoot
            if (Input.GetMouseButtonDown(0) && fireRateCD <= 0)
            {
                Shoot();
                fireRateCD = initFireRateCD;
            }

            // Repairs vehicle overtime when pressed near car
            if (canInteract && Input.GetKey(KeyCode.E) && currentInteractable == Interactable.Car
                && holdingRepairTool && !carBehaviour.isRepaired)
            {
                carBehaviour.isRepairing = true;
                sliderObject.SetActive(true);
                if (!audioSource.isPlaying) audioSource.Play();
            }
            else
            {
                carBehaviour.isRepairing = false;
                sliderObject.SetActive(false);
                audioSource.Pause();
            }

            // Look at Mouse
            float mousePosX = Input.mousePosition.x;
            float mousePosY = Input.mousePosition.y;
            float camDistance = cameraBehaviour.camDistance;

            Vector3 mousePos = mainCam.ScreenToWorldPoint(new Vector3(mousePosX, mousePosY, camDistance - 1f));
            transform.LookAt(mousePos);

            // Enter car when possible
            if (Input.GetKeyDown(KeyCode.E) && canInteract && currentInteractable == Interactable.Car
                && eventManager.carRepaired && !enterCar)
            {
                enterCar = true;
                GetComponent<Collider>().enabled = false;
                carBehaviour.ToggleCarSounds();
                eventManager.GetComponent<AudioSource>().Pause();
            }
            // If already entered, then exit car
            else if (Input.GetKeyDown(KeyCode.E) && enterCar)
            {
                enterCar = false;
                GetComponent<Collider>().enabled = true;
                carBehaviour.ToggleCarSounds();
                eventManager.GetComponent<AudioSource>().Play();

                transform.position = carExitPoint.transform.position;
            }

            if (interactCD < 0.3f) interactCD += Time.deltaTime;
            else canInteract = false;

            // Fix player go under ground when exiting car
            if (transform.position.y < 0f && !enterCar)
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }

            // When died
            // kasih transform tergeletak (blom)
            if (hp <= 0)
            {
                isDead = true;
                Destroy(gameObject);
            }

            // Shoot timer
            fireRateCD -= Time.deltaTime;

            // Show repair icon
            if (holdingRepairTool) repairIcon.SetActive(true);
            else repairIcon.SetActive(false);

            // Turns red when 1 hp
            if (hp == 1) animator.SetBool("low", true);
        }
    }

    private void LateUpdate()
    {
        // Fix bug aim kebawah. cause: sekitar CameraCenterPoint.cs
        Quaternion rotation = transform.rotation;
        rotation.x = 0f;        
    }

    // Shoot
    private void Shoot()
    {
        if (!enterCar)
        {
            Instantiate(bullet, position: bulletSpawnPos.transform.position, rotation: transform.rotation);
            Instantiate(shootAudioObject);
            shootParticleObject.GetComponent<ParticleSystem>().Play();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehaviour enemyBehaviour = collision.gameObject.GetComponent<EnemyBehaviour>();
            bool enemyDead = enemyBehaviour.dead;
            
            // Receive dmg after every 2s staying collided with enemy
            if (!enemyDead && !enemyBehaviour.stoleRepairTool) ReceiveDMG();
        }
    }

    
    public void ReceiveDMG()
    {
        if (!isReceivingDMG) StartCoroutine(ReceiveDMGTimer());
    }

    public IEnumerator ReceiveDMGTimer()
    {
        isReceivingDMG = true; // Checks if coroutine already running 
        hp--;
        animator.SetTrigger("hit");
        yield return new WaitForSeconds(2f);
        isReceivingDMG = false;
    }

    // Optimization / bug fix by giving a brief timer before allowing another interact.
    // Otherwise can conflict being inside/outside of car at the same time
    public void AllowInteract()
    {
        if (interactCD >= 0.3f)
        {
            canInteract = true;
            interactCD = 0f;
        }
    }
}
