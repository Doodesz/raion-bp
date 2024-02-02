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
    public EnemyType thisEnemyType;

    private bool spawned = false;
    private bool stunned = false;
    private float speed;
    private bool isRunning = false;

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
        transform.position = new Vector3(transform.position.x, -spawnOffset, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) // optimisasi MissingReferenceException setelah player destroyed
        {
            // Saat masih spawning
            if (!spawned)
            {
                transform.Translate(Vector3.up * spawnOffset * Time.deltaTime);
            }

            // Sesudah spawn
            else if (!dead && !stoleRepairTool)
            {
                if (!stunned)
                {
                    playerPos = new Vector3(player.transform.position.x, 1f, player.transform.position.z);
                    transform.LookAt(playerPos);
                }
                else if (stunned)
                {
                    transform.rotation = stunFacing;
                }

                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }

            // If dead
            else if (dead)
            {
                Invoke("Despawn", 3f);
            }

            else if (stoleRepairTool)
            {
                if (!isRunning)
                {
                    transform.localEulerAngles = GetRandomRotation(0);
                    isRunning = true;
                }
                else transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
                transform.Translate(Vector3.forward * speed * 2 * Time.deltaTime);
            }

            // Optimisasi
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < eventManager.renderDistance) GetComponent<Renderer>().enabled = true;
            else GetComponent<Renderer>().enabled = false;
        }
        else GetComponent<Renderer>().enabled = false;


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
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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

        if (collision.gameObject.CompareTag("Player") && thisEnemyType == EnemyType.Yippee && !dead)
        {
            StealRepairTool();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") && playerController.enterCar)
        {
            float carSpeed = Input.GetAxis("Vertical");
            if (carSpeed > 0.5f)
            {
                transform.Translate(new Vector3(0, 20f, 0) * Time.deltaTime);
                hp = 0;
                dead = true;
            }
        }

        if (stoleRepairTool)
        {
            if (!collision.gameObject.CompareTag("Ground"))
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSecondsRealtime(2);
        spawned = true;
        collider.enabled = true;
        rb.useGravity = true;
    }

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
        if (thisEnemyType == EnemyType.Normal)
        {
            animator.SetTrigger("hit");
        }
    }

    private void LowHPColor()
    {
        if (thisEnemyType == EnemyType.Normal)
        {
            animator.SetBool("low", true);
        }
    }

    // Fungsi buriq ga jelas tapi work hehe rraaaaaaaaahhh
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
}
