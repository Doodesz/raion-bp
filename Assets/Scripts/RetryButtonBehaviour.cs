using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RetryButtonBehaviour : MonoBehaviour
{
    private EventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (eventManager.gameEnded)
        {
            GetComponent<Image>().enabled = true;
            GetComponent<Button>().enabled = true;
            GetComponentInChildren<TMP_Text>().text = "Retry";
        }

        else
        {
            GetComponent<Image>().enabled = false;
            GetComponent<Button>().enabled = false;
            GetComponentInChildren<TMP_Text>().text = string.Empty;
        }

    }
}
