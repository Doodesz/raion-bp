using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderBehaviour : MonoBehaviour
{
    public float volume;

    private float maxVolume = 1f;

    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();    
    }

    // Update is called once per frame
    void Update()
    {
        volume = slider.value;
        slider.maxValue = maxVolume;
    }
}
