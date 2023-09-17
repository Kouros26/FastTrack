using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using System;
using UnityEngine.InputSystem;

//TODO : Stars panel. Check players scores (add all of them) and check.

[RequireComponent(typeof(StudioEventEmitter))]
public class GroupLife : MonoBehaviour
{
    [SerializeField] Slider lifeSlider;
    [SerializeField] float lifeDecay = 0.2f;

    [Tooltip("Life will be updated on the slider every x bits of life lost (life goes from 0 to 1)")]
    [SerializeField] float lifeDisplayThreshold = 0.1f;

    [Header("Sound Events :")]
    
    [SerializeField]
    [Tooltip("Sound event played when player loose the game.")]
    private StudioEventEmitter mSEventBadEnd;
    [SerializeField]
    [Tooltip("Sound event played when player win the game.")]
    private StudioEventEmitter mSEventGoodEnd;

    [Space(5)]

    [Header("Points Scriptable object")]
    public ScriptablePoints mPointScriptable;

    private bool mGameOverSequencePlaying = false;
    private RugsManager mRugManager;

    static Animator anim;

    static float life = 1.0f;
    static float currentLifeLost = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        life = 1.0f;
        mRugManager = FindAnyObjectByType<RugsManager>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        currentLifeLost += lifeDecay * Time.deltaTime;
        life -= lifeDecay * Time.deltaTime;

        //anim.Play("DecayIdle");

        if (currentLifeLost >= lifeDisplayThreshold)
        {
            lifeSlider.value = life;
            currentLifeLost = 0.0f;

            if (life <= 0)
            {
                MakeGameOver();
            }
        }
    }

    private bool GameOverSequenceIsPlaying()
    {
        return mGameOverSequencePlaying;
    }

    public void MakeGameOver()
    {
        mPointScriptable.mScore = 0;

        foreach (var player in FindAnyObjectByType<RugsManager>().playerList)
        {
            mPointScriptable.mScore += player.GetPoints();
        }

        if (!GameOverSequenceIsPlaying())
        {
            mRugManager.StopMusic();
            mGameOverSequencePlaying = true;

            if (life <= 0)
            {
                StartCoroutine("LooseEndScreen");
            }
            else
            {
                StartCoroutine("WinEndScreen");
            }
        }
    }

    IEnumerator LooseEndScreen()
    {
        mSEventBadEnd.Play();

        while (mSEventBadEnd.IsPlaying())
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(2); //Game over scene
    }

    IEnumerator WinEndScreen()
    {
        mSEventGoodEnd.Play();

        while (mSEventBadEnd.IsPlaying())
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(3); //Stars scene
    }

    public static void OnLifeChanged(float amount)
    {
        if (amount > 0)
            anim.Play("GainHP");

        else
            anim.Play("LoseHP");

        if (life + amount > 1)
        {
            life = 1;
            return;
        }

        life += amount;
    }
}
