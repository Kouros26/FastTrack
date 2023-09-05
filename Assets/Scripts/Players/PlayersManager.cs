using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    int playerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("PlayerInput id : " + playerInput.playerIndex);
        playerCount++;
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("PlayerLeft id : " + playerInput.playerIndex);
        playerCount--;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }
}
