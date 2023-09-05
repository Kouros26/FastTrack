using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    [SerializeField] UnityEngine.Object p1Text;
    [SerializeField] UnityEngine.Object p2Text;
    [SerializeField] UnityEngine.Object p3Text;
    [SerializeField] UnityEngine.Object startObject;
    [SerializeField] int startTextAlpha = 60;
    TMP_Text startText;

    int playerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        startText = startObject.GetComponent<TextMeshProUGUI>();

        startTextAlpha = Math.Clamp(startTextAlpha, 0, 255);

        startText.alpha = Alpha255to1(startTextAlpha);
    }

    public void OnClickedPlay()
    {
        if (AllPlayersConnected())
            SceneManager.LoadScene(1);
    }

    public void OnClickedQuit()
    {
        Application.Quit();
    }

    public void OnVolumeChanged()
    {
        AudioListener.volume = soundSlider.value;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("PlayerInput id : " + playerInput.playerIndex);
        playerCount++;

        switch (playerInput.playerIndex)
        {
            case 0:
                p1Text.GameObject().SetActive(true);
                break;

            case 1:
                p2Text.GameObject().SetActive(true);
                break;

            case 2:
                p3Text.GameObject().SetActive(true);
                break;
        }

        if (AllPlayersConnected())
            startText.alpha = 1;

    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("PlayerLeft id : " + playerInput.playerIndex);
        playerCount--;

        switch (playerInput.playerIndex)
        {
            case 0:
                p1Text.GameObject().SetActive(false);
                break;

            case 1:
                p2Text.GameObject().SetActive(false);
                break;

            case 2:
                p3Text.GameObject().SetActive(false);
                break;
        }

        if (!AllPlayersConnected())
            startText.alpha = startTextAlpha;
    }

    private bool AllPlayersConnected()
    {
        if (playerCount != 1) //TODO : Put this back to 3
            return false;

        return true;
    }

    private float Alpha255to1(int entry)
    {
        return entry / 255.0f;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }
}
