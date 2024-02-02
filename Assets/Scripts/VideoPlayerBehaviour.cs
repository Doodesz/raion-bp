using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerBehaviour : MonoBehaviour
{
    private Renderer _renderer;
    private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        videoPlayer = GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayWinClip()
    {
        videoPlayer.clip = Resources.Load("Nightcall") as VideoClip;
        videoPlayer.Play();
    }

    public void PlayDeadClip()
    {
        videoPlayer.clip = Resources.Load("Ded Clip") as VideoClip;
        videoPlayer.Play();
    }
}
