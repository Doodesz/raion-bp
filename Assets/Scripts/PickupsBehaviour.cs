using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PickupType { Repair, Shotgun };

public class PickupsBehaviour : MonoBehaviour
{
    public GameObject audioSourceObject;
    public PickupType thisPickupType;
    public GameObject interactablePromptObject;

    private GameObject player;
    private EventManager eventManager;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        player = GameObject.Find("Player");
        audioSource = audioSourceObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (thisPickupType == PickupType.Repair && !playerController.holdingRepairTool)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
                transform.position += new Vector3(0, -1, 0);
                Instantiate(audioSourceObject, transform.position, Quaternion.identity);
                playerController.holdingRepairTool = true;
                Destroy(gameObject);
            }
            else interactablePromptObject.SetActive(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactablePromptObject.SetActive(false);
        }
    }
}
