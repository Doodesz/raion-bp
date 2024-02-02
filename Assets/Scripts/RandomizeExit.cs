using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeExit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(-80f, 80f), 0f, -96.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // Would've add more exit sides but no time
    }
}
