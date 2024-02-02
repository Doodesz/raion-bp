using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class CarBehaviour : MonoBehaviour
{
    public bool isRepairing = false;
    public bool isRepaired = false;
    public float repairDuration = 30f;
    public float repairProgress = 0f;
    public bool enterCar = false;
    public float moveSpeed = 30f;
    public float steerPower = 10f;
    public GameObject hitAudioObject;
    public GameObject smokeParticle;
    public GameObject repairedSFXObject;
    public GameObject bgmObject;

    private float initMoveSpeed;
    private bool crashed = false;

    private GameObject player;
    private EventManager eventManager;
    private PlayerController playerController;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        initMoveSpeed = moveSpeed;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        enterCar = playerController.enterCar;

        if (repairProgress >= repairDuration)
        {
            eventManager.SubmitRepairTool();
        }

        if (isRepairing)
        {
            repairProgress += Time.deltaTime;
        }

        if (enterCar)
        {
            // Movement controls

            if (verticalAxis < 0 && !crashed)
            {
                horizontalAxis *= -1;
                moveSpeed = initMoveSpeed * 0.3f;
            }

            else if (!crashed)
            {
                moveSpeed = initMoveSpeed;
            }

            float turningAngle = Mathf.Abs(verticalAxis) * horizontalAxis * Time.deltaTime * moveSpeed * steerPower;

            // Turn right/left
            transform.Rotate(Vector3.up, turningAngle, Space.Self);
            // Move forward/backward
            transform.Translate(Vector3.forward * verticalAxis * moveSpeed * Time.smoothDeltaTime, Space.Self);
        }

        Quaternion carRot = transform.localRotation;

        if (transform.localRotation.z >= 40f)
        {
            transform.localRotation = Quaternion.Euler(carRot.x, carRot.y, 40f);
        }
        if (transform.localRotation.z <= -40f)
        {
            transform.localRotation = Quaternion.Euler(carRot.x, carRot.y, -40f);
        }

        audioSource.volume = GetComponent<VolumeController>().initVolume * verticalAxis;
        if (audioSource.volume < 0.1f) audioSource.volume = 0.1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float verticalAxis = Input.GetAxis("Vertical");

        if (collision.gameObject.CompareTag("Terrain"))
        {
            moveSpeed = 1f;
            crashed = true;

        }

        if (collision.gameObject.CompareTag("Exit"))
        {
            eventManager.Escaped();
        }

        if (collision.gameObject.CompareTag("Enemy") && playerController.enterCar && verticalAxis >= (1/2)*moveSpeed)
        {
            Instantiate(hitAudioObject, transform.position, Quaternion.identity);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            moveSpeed = initMoveSpeed;
            crashed = false;
        }
    }

    public void CarRepaired()
    {
        if (!eventManager.carRepaired)
        Instantiate(repairedSFXObject);

        Destroy(smokeParticle);
    }

    public void ToggleCarSounds()
    {
        // BGM
        AudioSource carBgmObject = bgmObject.GetComponent<AudioSource>();
        if (carBgmObject.isPlaying)
        {
            carBgmObject.Pause();
            ToggleEngineSound();
        }

        else
        {
            carBgmObject.Play();
            ToggleEngineSound();
        }
    }

    private void ToggleEngineSound()
    {
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.Play();
    }
}
