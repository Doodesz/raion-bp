using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    public float initVolume;

    private AudioSource audioSource;
    private EventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initVolume = audioSource.volume; // Gets GameObject original volume
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // When attached to audio source GameObjects, makes their volume the same on all other audio source objects.
        // At least within scene...
        audioSource.volume = initVolume * eventManager.gameVolume;
    }
}
