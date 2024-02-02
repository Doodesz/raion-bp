using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderBehaviour : MonoBehaviour
{
    public GameObject car;

    private CarBehaviour carBehaviour;
    private Slider slider;
    private PlayerController playerController;
    private EventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        carBehaviour = car.GetComponent<CarBehaviour>();
        slider = GetComponent<Slider>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = carBehaviour.repairDuration;
        slider.value = carBehaviour.repairProgress;

        if (slider.value >= slider.maxValue || carBehaviour.isRepaired)
        {
            gameObject.SetActive(false);
        }
    }
}
