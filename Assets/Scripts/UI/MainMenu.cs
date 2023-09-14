using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System;

//This would benefit from a total rewrite.
//Right now you can't really select a player with this.
public class MainMenu : MonoBehaviour
{
    [SerializeField] UnityEngine.Object startObject;
    [SerializeField] int startTextAlpha = 60;
    Image startImage;

    private int playerCount = 0;

    [Header("Game")]

    [SerializeField]
    [Tooltip("Numer of player required, at minimum, to start the game.")]
    private int mMinimumPlayers = 1;
    [SerializeField]
    [Tooltip("Numer of maximum players")]
    private int mMaxPlayers = 3;

    [Header("Lobby")]

    [Space(5)]

    [SerializeField]
    [Tooltip("Images that will be activated once the corresponding player join the game.")]
    private List<Image> mPlayersSprites = new List<Image>(3);

    [Space(5)]

    [SerializeField]
    [Tooltip("\"Press Start\n texts that will be de-activated once the corresponding player join the game.")]
    private List<TMP_Text> mStartTexts = new List<TMP_Text>(3);

    // Start is called before the first frame update
    void Start()
    {
        startImage = startObject.GetComponent<Image>();

        startTextAlpha = Math.Clamp(startTextAlpha, 0, 255);

        startImage.color = new Color(1, 1, 1, Alpha255to1(startTextAlpha));

        if (EnoughtPlayersConnected())
            startImage.color = new Color(1, 1, 1, 1);
    }

    public void OnClickedPlay()
    {
        if (EnoughtPlayersConnected())
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

        if (playerCount <= mMaxPlayers)
            ActivatePlayerSprite(playerCount);
        else
            Debug.LogWarning("Too many players ! Last joined will be ignored !");

        if (EnoughtPlayersConnected())
            startImage.color = new Color(1, 1, 1, 1);
    }

    private void ActivatePlayerSprite(int pPlayerIdx)
    {
        int listIdx = pPlayerIdx - 1;

        Image    sprite     = mPlayersSprites[listIdx];
        TMP_Text startText  = mStartTexts[listIdx];

        if (sprite == null || startText == null)
            return;

        sprite.gameObject.SetActive(true);
        startText.gameObject.SetActive(false);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("PlayerLeft id : " + playerInput.playerIndex);
        playerCount--;

        if (!EnoughtPlayersConnected())
            startImage.color = new Color(1, 1, 1, Alpha255to1(startTextAlpha));
    }

    private bool EnoughtPlayersConnected()
    {
        return playerCount >= mMinimumPlayers;
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
