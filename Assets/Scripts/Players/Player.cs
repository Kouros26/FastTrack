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
    private int badTimingPoints = 100;

    [SerializeField]
    [Tooltip("Amount of points awarded when hitting a note with \"ok\" timing.")]
    private int okTimingPoints = 300;

    [SerializeField]
    [Tooltip("Amount of points awarded when hitting a note with \"excellent\" timing.")]
    private int excellentTimingPoints = 500;


    [SerializeField]
    [Tooltip("Amount of points awarded when holding a note. (points being awarded per frame held).")]
    private int holdingPoints = 10;

    [Tooltip("Time, in second, between two ticks of points while holding a note.")]
    public float holdCooldown = 0.1f;
    
    private int mPlayerPoints = 0;

    private RugsManager mManager = null; 

    public void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void GivePoints(int pAmount)
    {
        mPlayerPoints += pAmount;
        Debug.Log(mPlayerPoints);
    }

    //Give points to player based on note stroke timing. Amount configurable in inspector.
    public void GivePoints(StrokeTiming pTiming)
    {
        switch (pTiming)
        {
            case StrokeTiming.Stroke_bad:
                mPlayerPoints += badTimingPoints;
                Debug.Log("bad : " + mPlayerPoints);
                break;

            case StrokeTiming.Stroke_ok:
                mPlayerPoints += okTimingPoints;
                Debug.Log("ok : " + mPlayerPoints);
                break;

            case StrokeTiming.Stroke_excellent:
                mPlayerPoints += excellentTimingPoints;
                Debug.Log("excellent : " + mPlayerPoints);
                break;

            case StrokeTiming.Stroke_holding:
                mPlayerPoints += holdingPoints;
                Debug.Log("Holding : " + mPlayerPoints);
                break;

            default:
                break;
        }
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
}
