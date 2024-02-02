using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    public float speed;
    private PlayButtonBehaviour playButtonBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        playButtonBehaviour = GameObject.Find("Play Button").GetComponent<PlayButtonBehaviour>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (!playButtonBehaviour.pressedPlay)
        transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
}
