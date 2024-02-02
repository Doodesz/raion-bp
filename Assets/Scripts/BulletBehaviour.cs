using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float speed = 5f;
    public float despawnTimer = 1f;
    public float knockbackPower = 15f;

    private Quaternion initRotation;

    // Start is called before the first frame update
    void Start()
    {
        initRotation = transform.rotation; // Should've used eulerAngles but whatevs
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = initRotation;
        StartCoroutine("Despawn");
    }

    // Despawn timer
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTimer);
        Destroy(gameObject);
    }

    // Destroys bullet when hitting with terrains/car. (Bullet also get destroyed when collided with enemy
    // but it's in their own script
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain") || collision.gameObject.CompareTag("Car"))
        {
            Destroy(gameObject);
        }
    }
}
