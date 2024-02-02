using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IndicatorType { Car, RepairTool, Exit };

public class ArrowIndicatorBehaviour : MonoBehaviour
{
    public float farDistance = 18f;
    public float nearDistance = 18f;
    public IndicatorType thisIndicatorType;
    public float timeToHint;

    public GameObject player;
    public GameObject arrow;

    private bool inRange = false;
    private bool showArrow = false;

    private GameObject repairTool;
    private GameObject[] repairTools;
    private GameObject car;
    private GameObject target;

    
    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("Car");    

    }

    private void Update()
    {
        if (player != null)
        {
            repairTools = GameObject.FindGameObjectsWithTag("Repair Tool");
        
            if (thisIndicatorType == IndicatorType.Car)
            {
                showArrow = true;

                if (inRange)
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = true;
                }
            
                target = car;
            }

            else if (thisIndicatorType == IndicatorType.RepairTool)
            {
                if(!showArrow) Invoke("ShowArrow", timeToHint);
                target = GetNearestRepair();
                arrow.GetComponent<SpriteRenderer>().enabled = true;

                if(target == null)
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = false;
                }
                else if (Vector3.Distance(player.transform.position, target.transform.position) < nearDistance)
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = false;
                }
                else if (inRange && showArrow)
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    arrow.GetComponent <SpriteRenderer>().enabled = false;
                }

            }

            transform.position = player.transform.position;
            if (target != null) // optimisasi menghilangkan exception no reference found
            {
                inRange = Vector3.Distance(player.transform.position, target.transform.position) < farDistance;
                Vector3 lookAtTarget = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                transform.LookAt(target.transform.position);
            }

        }

    }

    private void ShowArrow()
    {
        showArrow = true;
    }

    private GameObject GetNearestRepair()
    {
        GameObject[] repairTools = GameObject.FindGameObjectsWithTag("Repair Tool");
        float currentDistance = 10000f;
        int currentIndex = 0;

        for (int i = 0; i < repairTools.Length; i++)
        {
            float distance = Vector3.Distance(repairTools[i].transform.position, player.transform.position);
            if (distance < currentDistance)
            {
                currentIndex = i;
                currentDistance = distance;
            }
        }

        if (repairTools.Length > 0) return repairTools[currentIndex];
        else return null;
    }
}
