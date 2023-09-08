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
    [SerializeField] UnityEngine.Object startObject;
    [SerializeField] int startTextAlpha = 60;
    Image startImage;

    int playerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        startImage = startObject.GetComponent<Image>();

        startTextAlpha = Math.Clamp(startTextAlpha, 0, 255);

        startImage.color = new Color(1, 1, 1, Alpha255to1(startTextAlpha));

        if (AllPlayersConnected())
            startImage.color = new Color(1, 1, 1, 1);
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

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("PlayerInput id : " + playerInput.playerIndex);
        playerCount++;

        if (AllPlayersConnected())
            startImage.color = new Color(1, 1, 1, 1);

    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("PlayerLeft id : " + playerInput.playerIndex);
        playerCount--;

        if (!AllPlayersConnected())
            startImage.color = new Color(1, 1, 1, Alpha255to1(startTextAlpha));
    }

    private bool AllPlayersConnected()
    {
        if (playerCount != 3)
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
