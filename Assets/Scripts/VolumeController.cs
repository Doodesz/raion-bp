using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    public float initVolume;

    private VolumeSliderBehaviour volumeSliderBehaviour;
    private AudioSource audioSource;
    private EventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initVolume = audioSource.volume;
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = initVolume * eventManager.gameVolume;
    }
}
