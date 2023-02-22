using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PGGE.Multiplayer;

public class Menu : MonoBehaviour
{
    public GameObject mMultiplayerGameObject;

    ConnectionController connectionController;

    private void Start()
    {
        connectionController = mMultiplayerGameObject.GetComponent<ConnectionController>();
    }

    public void OnClickSinglePlayer()
    {
        StartCoroutine(PlayButtonSound());
    }

    public void OnClickMultiPlayer()
    {
        StartCoroutine(PlayButtonSound());
    }

    IEnumerator PlayButtonSound()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        AudioSource audioSource = button.GetComponent<AudioSource>();
        audioSource.Play();
        Debug.Log("audio start");
        yield return new WaitForSeconds(0.3f);
        audioSource.Stop();
        Debug.Log("audio end");

        switch (button.name)
        {
            // different function for different cases
            case "BtnSinglePlayer":
                LoadSinglePlayer();
                break;
            case "BtnMultiPlayer":
                LoadMultiplayer();
                break;
            case null:
                Debug.Log("Button pressed name is not in the case");
                break;
        }
    }

    private void LoadSinglePlayer()
    {
        Debug.Log("Loading Single Player Game");
    }

    private void LoadMultiplayer()
    {
        // load the multiplayer scene
        connectionController.mMenuCanvas.SetActive(false);
        connectionController.mMultiplayerCanvas.SetActive(true);
        Debug.Log("Loading Multiplayer Game");
    }
}
