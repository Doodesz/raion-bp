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
        if (player != null) // Optimization for no error after dying (Destroy player)
        {
            repairTools = GameObject.FindGameObjectsWithTag("Repair Tool");
        
            if (thisIndicatorType == IndicatorType.Car)
            {
                showArrow = true;
                target = car;

                if (inRange)
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = true;
                }
            }

            else if (thisIndicatorType == IndicatorType.RepairTool)
            {
                // Initial time before getting an arrow hinting to repair tools
                if(!showArrow) Invoke("ShowArrow", timeToHint);

                target = GetNearestRepair();

                if(target == null) // Optimization for when no repair tools are present (no object reference)
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = false;
                }

                // Disappears when very close (also fix bug arrow leaning down when too close)
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

                // Urutan if diatas penting (probs ada cara lebih efisien)
            }

            transform.position = player.transform.position;

            // Makes arrow looks towards target (car/repair)
            if (target != null) // Optimization no object reference error
            {
                inRange = Vector3.Distance(player.transform.position, target.transform.position) < farDistance;
                Vector3 lookAtTarget = new Vector3
                    (target.transform.position.x, transform.position.y, target.transform.position.z);
                transform.LookAt(target.transform.position);
            }
        }
    }

    private void ShowArrow()
    {
        showArrow = true;
    }
    
    // Makes arrow looks towards nearest repair tool
    private GameObject GetNearestRepair()
    {
        // Mengambil semua GameObject repair, menganggap jarak terdekat 10000f dan index repair 0
        GameObject[] repairTools = GameObject.FindGameObjectsWithTag("Repair Tool");
        float currentDistance = 10000f;
        int currentIndex = 0;

        // Mencari repair tool terdekat dengan membandingkan jarak mereka dengan player satu-persatu
        for (int i = 0; i < repairTools.Length; i++)
        {
            float distance = Vector3.Distance(repairTools[i].transform.position, player.transform.position);
            if (distance < currentDistance)
            {
                currentIndex = i;
                currentDistance = distance;
            }
        }

        /* Was to fix bug when there are no repairs present but not anymore since repairs always respawns
        whenever one of them disappears (taken by player). Check SpawnManager script: Is a gameplay change 
        from taking all repairs before fixing car to taking repairs then fix one by one*/
        if (repairTools.Length > 0) return repairTools[currentIndex];
        else return null;
    }
}
