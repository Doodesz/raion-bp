using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public enum EnemyType { Normal, Yippee };

public class EnemyBehaviour : MonoBehaviour
{
    public int hp;
    public float initSpeed;
    public float spawnOffset = 1f;
    public bool dead = false;
    public bool stoleRepairTool = false;

    public GameObject yippeeAudioObject;
    public GameObject hitAudioObject;
    public GameObject stolenSFXObject;
    public GameObject wrenchModelObject;
    public EnemyType thisEnemyType;

    private bool spawned = false;
    private bool stunned = false;
    private float speed;
    private bool isRunning = false;
    private float audioSpawnCD = 0.5f;

    private Vector3 playerPos;
    private Quaternion stunFacing;
    private GameObject player;
    private Collider collider;
    private Rigidbody rb;
    private EventManager eventManager;
    private Animator animator;
    private PlayerController playerController;
    private AnnounceTextBehaviour announceTextBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnTimer());

        player = GameObject.Find("Player");
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        eventManager = GameObject.Find("Event Manager").GetComponent<EventManager>();
        animator = GetComponent<Animator>();
        playerController = player.GetComponent<PlayerController>();
        announceTextBehaviour = GameObject.Find("Announcer Text").GetComponent<AnnounceTextBehaviour>();

        speed = initSpeed;
        collider.enabled = false;
        rb.useGravity = false;

        // Rise from ground anim init
        transform.position = new Vector3(transform.position.x, -spawnOffset, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) // MissingReferenceException when player destroyed optimization
        {
            // When spawning, appears from ground gradually
            if (!spawned)
            {
                transform.Translate(Vector3.up * spawnOffset * Time.deltaTime);
            }

            // Post-spawn
            else if (!dead && !stoleRepairTool)
            {
                // Looks at player when not stunned
                if (!stunned)
                {
                    playerPos = new Vector3(player.transform.position.x, 1f, player.transform.position.z);
                    transform.LookAt(playerPos);
                }

                // Freeze rotation when stunned
                else if (stunned)
                {
                    transform.rotation = stunFacing;
                }

                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }

            // Despawns in a set time after dying
            else if (dead)
            {
                Invoke("Despawn", 3f);
            }

            // Changes direction after stealing a repair tool
            else if (stoleRepairTool)
            {
                if (!isRunning)
                {
                    transform.eulerAngles = GetRandomRotation(0);
                    isRunning = true;
                }
                else transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                transform.Translate(Vector3.forward * speed * 2 * Time.deltaTime);
            }

            // Optimization to unrender when out of camera
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < eventManager.renderDistance) GetComponent<Renderer>().enabled = true;
            else GetComponent<Renderer>().enabled = false;
        }

        else GetComponent<Renderer>().enabled = false; // Optimization/bug fix when player died (destroyed)
                                                       // no gameobject reference


        // Destroy when out of Bounds
        if (transform.position.y < -10f || transform.position.x > 110f || transform.position.x < -110f || transform.position.z > 110f || transform.position.z < -110f)
        {
            Destroy(gameObject);
        }
        if (transform.position.y > 2f)
        {
            transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
        }

        if (hp <= 1)
        {
            LowHPColor();
        }

        if (hp <= 0)
        {
            dead = true;
            animator.SetBool("isDead", true);
            rb.useGravity = true; // For yippees to fall down
        }

        // Optimize instantiated audio hits
        audioSpawnCD -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Knocks away and stuns enemy when hitting a bullet. However, if died already, ignores bullet
        if (collision.gameObject.CompareTag("Bullet") && !dead)
        {
            Vector3 bulletPos = collision.gameObject.transform.position;
            Vector3 knockDir = transform.position - bulletPos;
            float knockback = collision.gameObject.GetComponent<BulletBehaviour>().knockbackPower;

            StartCoroutine(Stun(1));
            Destroy(collision.gameObject);
            hp--;
            rb.AddForce(-knockDir * knockback, ForceMode.Impulse);
            Instantiate(hitAudioObject, transform.position, Quaternion.identity);
        }

        // If thisEnemyType is Yippee, steals repair tool
        if (collision.gameObject.CompareTag("Player") && thisEnemyType == EnemyType.Yippee && !dead)
        {
            StealRepairTool();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        float carSpeed = Input.GetAxis("Vertical");
        
        // Kills instantly and knocks enemy up when getting hit by car at high speed
        if (collision.gameObject.CompareTag("Car") && playerController.enterCar && carSpeed > 0.5f)
        {
            if (carSpeed > 0.5f)
            {
                transform.Translate(new Vector3(0, 20f, 0) * Time.deltaTime);
                hp = 0;
                dead = true;
                SpawnHitAudioObject();
            }
        }

        // Fix bug Yippees going below ground after stealing
        if (stoleRepairTool)
        {
            if (!collision.gameObject.CompareTag("Ground"))
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
    }

    // Spawn timer before chasing player
    IEnumerator SpawnTimer()
    {
        yield return new WaitForSecondsRealtime(2);
        spawned = true;
        collider.enabled = true;
        rb.useGravity = true;
    }

    // When being hit by bullet, stuns them for brief moment, giving player some breath
    IEnumerator Stun(float duration)
    {
        stunned = true;
        speed = 0f;
        stunFacing = transform.rotation;
        Hit();

        yield return new WaitForSecondsRealtime(duration);

        stunned = false;
        speed = initSpeed;
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private void Hit()
    {
        // Normal enemy-specific because there's no hit trigger for yippees and im lazy and forgor to add
        if (thisEnemyType == EnemyType.Normal) 
        {
            animator.SetTrigger("hit");
        }
    }

    private void LowHPColor()
    {
        // Also // Normal enemy-specific because there's no hit trigger for yippees and im lazy and forgor to add
        if (thisEnemyType == EnemyType.Normal)
        {
            animator.SetBool("low", true);
        }
    }

    // Fungsi nyuri repair tool buriq ga jelas tapi work hehe rraaaaaaaaahhh
    private void StealRepairTool()
    {
        // Urutan eksekusi penting disini
        if (!stoleRepairTool)
        {
            stoleRepairTool = true;

            if (!playerController.holdingRepairTool) playerController.ReceiveDMG();
            else
            {
                announceTextBehaviour.AnnounceItemStolen("Repair Tool");
                Instantiate(stolenSFXObject);
                wrenchModelObject.SetActive(true);
            }
            
            playerController.holdingRepairTool = false;
            speed = 15f;
        }
        Instantiate(yippeeAudioObject);
    }

    private Vector3 GetRandomRotation(float xRot)
    {
        float randomAngle = Random.Range(0f, 360f);
        Vector3 random = new Vector3(0,randomAngle,0);
        return random;
    }

    private void SpawnHitAudioObject()
    {
        if (audioSpawnCD <= 0)
        {
            Instantiate(hitAudioObject, transform.position, Quaternion.identity);
            audioSpawnCD = 0.5f;
        }
    }
}
