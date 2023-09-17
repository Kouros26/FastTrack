using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] InGameManager gameManager;

    private bool buttonOne = false;
    private bool buttonTwo = false;
    private bool buttonThree = false;

    private bool buttonOneHeld = false;
    private bool buttonTwoHeld = false;
    private bool buttonThreeHeld = false;
    private bool buttonFourHeld = false;
    
    [Header("Points")]

    [SerializeField]
    [Tooltip("Amount of points awarded when hitting a note with \"bad\" timing.")]
    private float missedTimingPoints = -0.1f;

    [SerializeField]
    [Tooltip("Amount of points awarded when hitting a note with \"bad\" timing.")]
    private float badTimingPoints = 0.1f;

    [SerializeField]
    [Tooltip("Amount of points awarded when hitting a note with \"ok\" timing.")]
    private float okTimingPoints = 0.3f;

    [SerializeField]
    [Tooltip("Amount of points awarded when hitting a note with \"excellent\" timing.")]
    private float excellentTimingPoints = 0.5f;

    [SerializeField]
    [Tooltip("Amount of points awarded when holding a note. (points being awarded per frame held).")]
    private float holdingPoints = 0.01f;

    [SerializeField]
    [Tooltip("Percentage point boost when under group bonus.")]
    private int mPointBonusPercentage = 100;

    [Tooltip("Time, in second, between two ticks of points while holding a note.")]
    public float holdCooldown = 0.1f;

    private int mPlayerPoints = 0;

    private RugsManager mManager = null; 

    private bool mPointsBonusActive = false;
    public int GetPoints()
    {
        return mPlayerPoints;
    }

    public void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private int BonusModificator(int pPoints)
    {
        return pPoints + (pPoints * (mPointBonusPercentage / 100));
    }

    public void GivePoints(int pAmount)
    {

    }

    public void EnableBonus()
    {
        mPointsBonusActive = true;
        Debug.Log("Bonus enabled ! : score * x%");
    }

    public void DisableBonus()
    {
        mPointsBonusActive = false;
        Debug.Log("End of bonus !");
    }

    //Give points to player based on note stroke timing. Amount configurable in inspector.
    public void GivePoints(StrokeTiming pTiming)
    {
        int pointsGiven = 0;
        switch (pTiming)
        {
            case StrokeTiming.Stroke_bad:
                pointsGiven = (int)(badTimingPoints);
                Debug.Log("BAD !");
                GroupLife.OnLifeChanged(badTimingPoints);
                break;

            case StrokeTiming.Stroke_ok:
                pointsGiven = (int)(okTimingPoints);
                Debug.Log("OK");
                GroupLife.OnLifeChanged(okTimingPoints);
                break;

            case StrokeTiming.Stroke_excellent:
                GroupLife.OnLifeChanged(excellentTimingPoints);
                pointsGiven = (int)(excellentTimingPoints);
                Debug.Log("Excellent");
                break;

            case StrokeTiming.Stroke_holding:
                pointsGiven = (int)(holdingPoints);
                Debug.Log("Holding");
                GroupLife.OnLifeChanged(holdingPoints);
                break;

            default:
                GroupLife.OnLifeChanged(missedTimingPoints);
                break;
        }

        if (mPointsBonusActive)
            pointsGiven = BonusModificator(pointsGiven);

        mPlayerPoints += pointsGiven;
        Debug.Log("Points : "+ mPlayerPoints);
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        gameManager = FindObjectOfType<InGameManager>(GameObject.Find("GameManager"));

        if (gameManager == null)
            Debug.Log("Game Manager not found");

        //Join rug
        mManager = FindFirstObjectByType<RugsManager>();

        if (mManager == null)
        {
            Debug.LogError("Player : Couldnt Find Rug Manager !!");
            return;
        }
        else
        {
            mManager.AssignRug(this);
        }
        //------------
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Start();
    }

    public void OnButtonOne(InputAction.CallbackContext context)
    {
        buttonOne = context.action.triggered;
    }

    public void OnButtonTwo(InputAction.CallbackContext context)
    {
        buttonTwo = context.action.triggered;
    }

    public void OnButtonThree(InputAction.CallbackContext context)
    {
        buttonThree = context.action.triggered;
    }

    public void OnButtonOneHeld(InputAction.CallbackContext context)
    {
        buttonOneHeld = context.action.triggered;
    }

    public void OnButtonTwoHeld(InputAction.CallbackContext context)
    {
        buttonTwoHeld = context.action.triggered;
    }

    public void OnButtonThreeHeld(InputAction.CallbackContext context)
    {
        buttonThreeHeld = context.action.triggered;
    }

    public void OnPauseTriggered(InputAction.CallbackContext context)
    {
        gameManager.OnPause();
    }

    internal void ResetScore()
    {
        mPlayerPoints = 0;
    }
}
