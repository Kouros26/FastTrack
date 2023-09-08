using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using System;

//TODO : Stars panel. Check players scores (add all of them) and check.

[RequireComponent(typeof(StudioEventEmitter))]
public class GroupLife : MonoBehaviour
{
    [SerializeField] Slider lifeSlider;
    [SerializeField] float lifeDecay = 0.2f;

    [Tooltip("Life will be updated on the slider every x bits of life lost (life goes from 0 to 1)")]
    [SerializeField] float lifeDisplayThreshold = 0.1f;
    //[SerializeField] float mistakesDamage = 0.1f;
    //Combos aren't implemented yet

    static float life = 1.0f;
    static float currentLifeLost = 0.0f;

    [Header("Sound Events :")]
    [SerializeField]
    [Tooltip("Sound event played when player loose the game.")]
    private StudioEventEmitter mSEventBadEnd;
    
    private bool mGameOverSequencePlaying = false;

    private RugsManager mRugManager;

    // Start is called before the first frame update
    void Start()
    {
        life = 1.0f;
        mRugManager = FindAnyObjectByType<RugsManager>();
    }

    private void Update()
    {
        currentLifeLost += lifeDecay * Time.deltaTime;
        life -= lifeDecay * Time.deltaTime;

        if (currentLifeLost >= lifeDisplayThreshold)
        {
            lifeSlider.value = life;
            currentLifeLost = 0.0f;

            if (life <= 0 && !GameOverSequenceIsPlaying())
            {
                StartCoroutine("EndSequence");
            }
        }
    }

    private bool GameOverSequenceIsPlaying()
    {
        return mGameOverSequencePlaying;
    }

    IEnumerator EndSequence()
    {
        mGameOverSequencePlaying = true;
        
        mRugManager.StopMusic();
        mSEventBadEnd.Play();

        while (mSEventBadEnd.IsPlaying())
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(2); //Game over scene
    }

    public static void OnLifeChanged(float amount)
    {
        if (life + amount > 1)
        {
            life = 1;
            return;
        }

        life += amount;
    }
}
