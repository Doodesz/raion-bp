using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldedItem { Repair };

public class HoldedItemBehaviour : MonoBehaviour
{
    public HoldedItem thisItem;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (thisItem == HoldedItem.Repair && playerController.holdingRepairTool)
        {
            GetComponent<Renderer>().enabled = true;
        }
        else GetComponent<Renderer>().enabled = false;

        // Used enum so I can easily add more interactables without adding more scripts for the shown held object
    }
}
