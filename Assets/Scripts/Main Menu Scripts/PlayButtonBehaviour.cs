using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonBehaviour : MonoBehaviour
{
    public bool pressedPlay = false;
    public GameObject explosionParticle;

    // Start is called before the first frame update
    void Start()
    {
        pressedPlay = false;
        GetComponent<AudioSource>().Stop();
        explosionParticle.SetActive(false);
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play()
    {
        Invoke("OnPressingPlay",5f);
        explosionParticle.SetActive(true);

        if (!pressedPlay)
        GetComponent<AudioSource>().Play();
        
        pressedPlay = true;
    }

    private void OnPressingPlay()
    {
        SceneManager.LoadScene("Play Scene");
    }
}
