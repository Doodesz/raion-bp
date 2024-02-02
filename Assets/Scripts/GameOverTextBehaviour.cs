using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverTextBehaviour : MonoBehaviour
{
    private EventManager eventManager;
    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (eventManager.gameWon)
        {
            text.text = "You Drive!";
        }

        else if (eventManager.playerDied)
        {
            text.text = "You're Dead x(";
        }

        else text.text = "";
    }
}
