using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Interactable { Car };

public class EventManager : MonoBehaviour
{
    public int repairedParts = 0;
    public float promptDistance = 6f;
    public bool carRepaired = false;
    public bool gameWon = false;
    public bool playerDied = false;
    public bool gameEnded = false;
    public float renderDistance = 30f;
    public float gameVolume = 0.7f;
    
    public GameObject repairCounter;
    public GameObject player;
    public GameObject car;
    public GameObject carRepairPrompt;
    public GameObject videoPlayer;
    public GameObject videoRenderer;
    public GameObject pauseScreen;

    public Interactable currentInteractable = Interactable.Car;

    private string currentScene;

    private AudioSource audioSource;
    private TMP_Text carPromptText;
    private PlayerController playerController;
    private TMP_Text repairTextInput;
    private CarBehaviour carBehaviour;
    private VideoPlayerBehaviour videoPlayerBehaviour;
    private VolumeSliderBehaviour volumeSliderBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        repairTextInput = repairCounter.GetComponent<TMP_Text>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        carPromptText = carRepairPrompt.GetComponent<TMP_Text>();
        carBehaviour = car.GetComponent<CarBehaviour>();
        videoPlayerBehaviour = videoPlayer.GetComponent<VideoPlayerBehaviour>();
        audioSource = GetComponent<AudioSource>();
        currentScene = SceneManager.GetActiveScene().name;
        videoRenderer.GetComponent<RawImage>().enabled = false;
        Physics.gravity *= 3f; // For GameObjects to be heavier and harder for car to flip (possible anyways)
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) // Optimization
        {
            // Show prompts for interactables (car)
            if(Vector3.Distance(player.transform.position, car.transform.position) < promptDistance && !playerController.enterCar)
            {
                carPromptText.enabled = true;
                playerController.AllowInteract();
                currentInteractable = Interactable.Car;
            }
            else
            {
                carPromptText.enabled = false;
            }

            // Monitors if car is fixed
            if(repairedParts >= 3)
            {
                carBehaviour.CarRepaired(); // To call fixed sfx
                carRepaired = true;
            }

            // Calls dead()
            if (playerController.isDead && !gameEnded)
            {
                Died();
            }
        }

        // Gets volumeSliderBehaviour on pause
        if (pauseScreen.activeSelf == true)
        volumeSliderBehaviour = GameObject.Find("Volume Slider").GetComponent<VolumeSliderBehaviour>();

        // Otherwise stays the same. Also an optimization/bug fix for when pause screen is disabled
        if (volumeSliderBehaviour != null)
        {
            gameVolume = volumeSliderBehaviour.volume;
        }
    }

    // Texts are put here to avoid hiccups for a split-second before changing text
    private void LateUpdate()
    {
        if (player != null)
        {
            // Manages prompt text
            if (!carRepaired && !playerController.holdingRepairTool) carPromptText.text = "Find a Repair Tool to repair";
            else if (!carRepaired && playerController.holdingRepairTool) carPromptText.text = "Repair\n[Hold E]";
            else if (carRepaired) carPromptText.text = "Drive [E]";
        
            // Manages instruction text
            string textInput = string.Empty;
            if (playerController.holdingRepairTool && !carRepaired) textInput = "Repair Car";
            else if (repairedParts < 3 && !carRepaired) textInput = "Find Repair Tools (" + repairedParts.ToString() + "/3)";
            else if (carRepaired && !playerController.enterCar) textInput = "Drive";
            else if (playerController.enterCar) textInput = "Find Exit!";
            else textInput = "Error no text input conditions met";

            if (playerController.hp <= 2) textInput = "You're hurt\n" + textInput;

            repairTextInput.text = textInput;
        }
    }

    // Shows a video of ryan gosling upon winning
    public void Escaped()
    {
        videoPlayerBehaviour.PlayWinClip();
        videoRenderer.GetComponent<RawImage>().enabled = true;
        audioSource.Pause();
        gameWon = true;
        gameEnded = true;
    }

    // Shows a cringe cat upon dying
    private void Died()
    {
        videoPlayerBehaviour.PlayDeadClip();
        videoRenderer.GetComponent<RawImage>().enabled = true;
        audioSource.Pause();
        playerDied = true;
        gameEnded = true;
    }

    // Increases repair tool count and resets fixing bar
    public void SubmitRepairTool()
    {
        repairedParts++;
        carBehaviour.repairProgress = 0f;
        playerController.holdingRepairTool = false;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(currentScene);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
        Physics.gravity /= 3f;
    }
}
