using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScoreUI : MonoBehaviour
{
    [Header("UI Elements : ")]

    [SerializeField]
    [Tooltip("Text that will be updated with the score of the guitarist.")]
    private TMP_Text mGuitarText;
    [SerializeField]
    [Tooltip("Text that will be updated with the score of the drummer.")]
    private TMP_Text mDrumText;
    [SerializeField]
    [Tooltip("Text that will be updated with the score of the bassist player.")]
    private TMP_Text mBassText;


    private RugsManager mRugManager;

    // Start is called before the first frame update
    void Start()
    {
        mRugManager = FindAnyObjectByType<RugsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore(mGuitarText, mRugManager.GetGuitarScore());
        UpdateScore(mDrumText, mRugManager.GetDrumScore());
        UpdateScore(mBassText, mRugManager.GetBassScore());
    }

    private void UpdateScore(TMP_Text pScoreText, int pScore)
    {
        //No player
        if (pScore < 0)
            pScoreText.text = "Press start to join !";
        else
            pScoreText.text = "Score : " + pScore;
    }
}
