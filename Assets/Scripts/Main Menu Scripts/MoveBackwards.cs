using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackwards : MonoBehaviour
{
    public float speed = 20f;

    private PlayButtonBehaviour playButtonBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        playButtonBehaviour = GameObject.Find("Play Button").GetComponent<PlayButtonBehaviour>();
        speed = 7f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.smoothDeltaTime);

        if (transform.position.z < -40)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 30f);
        }

        if (playButtonBehaviour.pressedPlay)
        {
            speed = 0f;
        }
    }
}
