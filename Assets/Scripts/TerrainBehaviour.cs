using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType { Tree, Building };

public class TerrainBehaviour : MonoBehaviour
{
    public TerrainType thisTerrainType;
    public GameObject treeHitAudioSourceObject;

    private bool isBroken = false;
    private bool playingSound = false;

    private GameObject player;
    private Rigidbody rb;
    private EventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (thisTerrainType == TerrainType.Tree)
        {
            if (isBroken) FallDown();
        }
        
        if (player != null)
        {
            // Optimisasi
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < eventManager.renderDistance) GetComponent<Renderer>().enabled = true;
            else GetComponent<Renderer>().enabled = false;
        }
        else GetComponent<Renderer>().enabled =false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") && thisTerrainType == TerrainType.Tree)
        {
            isBroken = true;
            if (!playingSound) StartCoroutine(PlayTreeHitSound());
        }

        if (collision.gameObject.CompareTag("Terrain") || collision.gameObject.CompareTag("Repair Tool"))
        {
            Vector3 newSpawnPos = GameObject.Find("Spawner").GetComponent<SpawnManager>().GetNewSpawnPos(1.5f);
            transform.position = newSpawnPos;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") && thisTerrainType == TerrainType.Tree)
        {
            transform.Translate(Vector3.up * 3f * Time.smoothDeltaTime);
        }
    }

    private void FallDown()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.freezeRotation = false;
    }

    private IEnumerator PlayTreeHitSound()
    {
        Instantiate(treeHitAudioSourceObject);
        playingSound = true;
        yield return new WaitForSecondsRealtime(0.5f);
        playingSound = false;
    }
}
