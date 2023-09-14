using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class StrokingArea : MonoBehaviour
{
    [Header("Stroke Timings")]
    [SerializeField]
    [Range(0.0f, 5f)]
    private float badTimingOffset = 0.5f;
    [SerializeField]
    [Range(0.0f, 5f)]
    private float okTimingOffset = 0.3f;
    [SerializeField]
    [Range(0.0f, 5f)]
    private float excellentTimingOffset = 0.1f;

    public UnityEngine.Object failPrefab;
    [SerializeField] UnityEngine.Object okPrefab;
    [SerializeField] UnityEngine.Object excellentPrefab;
    [SerializeField] UnityEngine.Object badPrefab;

    public SpriteRenderer spriteRenderer;
    public Sprite idle;
    public Sprite success;
    public Sprite failure;

    PlayerInput playerinput;

    [Header("Inputs")]
    [SerializeField]
    [Tooltip("The name of the action this stroking area is looking for. (see player input actions).")]
    private string ActionName = "";

    private Note mNoteHeld = null;      //The note currently held. Null if none.
    private Note mHeldBonusNote = null; //The bonus note currently held. Null if none.

    private Player mPlayerRef = null;
    private RugTrack mTrack = null;
    private InputAction mPlayerInputAction = null;

    [HideInInspector]
    public float holdCooldownTimer = 0; //For hold points tick cooldown;

    [Header("Visuals")]
    [SerializeField]
    private Animator mAreaAnimator;    

    public void SetControllingPLayer(Player pPlayer)
    {
        if (pPlayer == null) return;

        mPlayerRef = pPlayer;

        mPlayerInputAction = mPlayerRef.GetComponent<PlayerInput>().actions[ActionName];
        mPlayerInputAction.performed += ActionPerformed;
        mPlayerInputAction.canceled += ActionCanceled;
    }

    private void StopHolding()
    {
        mNoteHeld.isHeld = false;
        mNoteHeld.isStroked = true;
        mAreaAnimator.SetBool("Hold", false);
        ResetSprite();
    }

    //Button released
    public void ActionCanceled(InputAction.CallbackContext context)
    {
        if (!context.canceled) return;

        if(mNoteHeld != null) 
        {
            StopHolding();
        }

        if (mHeldBonusNote != null)
        {
            mHeldBonusNote.isStroked = true;
            GetRugManager().BonusIsReleased();
        }

        mHeldBonusNote = null;
    }
    
    //Button pressed
    public void ActionPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return; 
        }

        mAreaAnimator.SetTrigger("Pressed");

        foreach (Note note in mTrack.GetNotesOnTrack())
        {
             if (note == null) continue;

            //Note can't be touched anymore.
            bool isInRangeOfZone = note.transform.position.y <= this.transform.position.y + badTimingOffset && note.transform.position.y > this.transform.position.y;

            if (!isInRangeOfZone)
                continue;

            switch (note.mType)
            {
                case NoteType.Note_Hold:
                    StartHoldingNote(note);
                    return;

                case NoteType.Note_Stroke:
                    StrikeNote(note);
                    return;

                case NoteType.Note_BonusShard:
                    StartHoldingBonus(note);
                    return;

                default:
                    return;
            }
        }
    }

    private void StartHoldingBonus(Note note)
    {
        mHeldBonusNote = note;
        note.gameObject.transform.position = this.transform.position; //Snap note to stroke area        
        GetRugManager().NewBonusIsHeld();   
    }

    private RugsManager GetRugManager()
    {
        NoteRug rug = mTrack.GetRug();
        RugsManager rugManager = rug.GetManager();

        return rugManager;
    }

    private void StartHoldingNote(Note pNote)
    {
        if (mNoteHeld != null) return; //Two notes at the same time ? nonono.

        mAreaAnimator.SetBool("Hold", true);
        mAreaAnimator.ResetTrigger("Pressed");

        mNoteHeld = pNote;
        mNoteHeld.isHeld = true;
        mNoteHeld.holdTimer = 0;
    }

    public bool NoteIsHeld()
    {
        return mNoteHeld != null;
    }

    private void Update()
    {
        if (NoteIsHeld())
        {
            //Should give points ?
            if (holdCooldownTimer >= mPlayerRef.holdCooldown)
            {
                holdCooldownTimer = 0;
                mPlayerRef.GivePoints(StrokeTiming.Stroke_holding);
                Instantiate(excellentPrefab, gameObject.transform);
            }

            holdCooldownTimer += Time.deltaTime;
            mNoteHeld.holdTimer += Time.deltaTime;

            if (mNoteHeld.holdTimer >= mNoteHeld.timeToHold) 
                StopHolding();
        }
    }

    private void StrikeNote(Note note)
    {
        note.isStroked = true;

        bool isInBadTiming = note.transform.position.y >= this.transform.position.y + okTimingOffset;

        ResetSprite();

        if (isInBadTiming)
        {
            Instantiate(badPrefab, gameObject.transform);
            mPlayerRef.GivePoints(StrokeTiming.Stroke_bad);
            spriteRenderer.sprite = success;
            Invoke("ResetSprite", 0.5f);
            return;
        }

        bool isInOkTiming = note.transform.position.y >= this.transform.position.y + excellentTimingOffset;

        if (isInOkTiming)
        {
            mPlayerRef.GivePoints(StrokeTiming.Stroke_ok);
            Instantiate(okPrefab, gameObject.transform);
            spriteRenderer.sprite = success;
            Invoke("ResetSprite", 0.5f);
            return;
        }

        bool isInExcellentTiming = note.transform.position.y >= this.transform.position.y;

        if (isInExcellentTiming)
        {
            mPlayerRef.GivePoints(StrokeTiming.Stroke_excellent);
            Instantiate(excellentPrefab, gameObject.transform);
            spriteRenderer.sprite = success;
            Invoke("ResetSprite", 0.5f);
            return;
        }

        mPlayerRef.GivePoints(StrokeTiming.Stroke_missed);
        Instantiate(failPrefab);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one);

        const float timingSphereSize = 0.025f;

        Vector3 badtimingSpherePosition = transform.position;
        Vector3 oktimingSpherePosition = transform.position;
        Vector3 excellenttimingSpherePosition = transform.position;

        badtimingSpherePosition.y += badTimingOffset;
        oktimingSpherePosition.y += okTimingOffset;
        excellenttimingSpherePosition.y += excellentTimingOffset;

        //Bad timing zone
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(badtimingSpherePosition, timingSphereSize);
        Gizmos.DrawLine(badtimingSpherePosition, oktimingSpherePosition);

        //Ok Timing zone
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(oktimingSpherePosition, excellenttimingSpherePosition);
        Gizmos.DrawSphere(oktimingSpherePosition, timingSphereSize);

        //Excellent Timing zone
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(excellenttimingSpherePosition, this.transform.position);

        Gizmos.DrawSphere(excellenttimingSpherePosition, timingSphereSize);

        //Center
        Gizmos.color = Color.black;
        Gizmos.DrawCube(transform.position, new Vector3(timingSphereSize, timingSphereSize, timingSphereSize));
    }

    public void SetTrack(RugTrack pRugTrack)
    {
        mTrack = pRugTrack;
    }

    internal bool IsHoldingBonus()
    {
        return mHeldBonusNote != null;
    }

    public void ResetSprite()
    {
        spriteRenderer.sprite = idle;
    }
}
