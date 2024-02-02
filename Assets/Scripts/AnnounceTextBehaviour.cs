using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnounceTextBehaviour : MonoBehaviour
{
    private TMP_Text tmpText;
    private Animator animator;

    private string text;

    // Start is called before the first frame update
    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        tmpText.text = text;
    }

    public void AnnounceItemStolen(string item)
    {
        animator.SetTrigger("show");
        text = item + " Stolen!";
    }

    // unused
    public void AnnounceCarNeeded()
    {
        animator.SetTrigger("show");
        text = "Use Your Car to Continue";
    }

    public void AnnounceLowHP()
    {
        text = "Current HP Low";
        animator.SetTrigger("show");

    }
}
