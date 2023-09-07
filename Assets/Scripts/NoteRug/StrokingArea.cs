using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class StrokingArea : MonoBehaviour
{
    [Header("Stroke Timings")]
    [SerializeField] private float badTimingOffset = 0.5f;
    [SerializeField] private float okTimingOffset = 0.3f;
    [SerializeField] private float excellentTimingOffset = 0.1f;

    [SerializeField] UnityEngine.Object failPrefab;
    [SerializeField] UnityEngine.Object okPrefab;
    [SerializeField] UnityEngine.Object excellentPrefab;

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
    

    public void SetControllingPLayer(Player pPlayer)
    {
        if (pPlayer == null) return;

        mPlayerRef = pPlayer;

        mPlayerInputAction = mPlayerRef.GetComponent<PlayerInput>().actions[ActionName];
        mPlayerInputAction.performed += ActionPerformed;
        mPlayerInputAction.canceled += ActionCanceled;
    }

    //Button released
    public void ActionCanceled(InputAction.CallbackContext context)
    {
        if (!context.canceled) return;

        if(mNoteHeld != null) { mNoteHeld.isStroked = true; }

        if (mHeldBonusNote != null)
        {
            mHeldBonusNote.isStroked = true;
            GetRugManager().BonusIsReleased();
        }

        mHeldBonusNote = null;
        mNoteHeld = null;
    }
    
    //Button pressed
    public void ActionPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return; 
        }

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
                    continue;

                case NoteType.Note_Stroke:
                    StrikeNote(note);
                    continue;

                case NoteType.Note_BonusShard:
                    StartHoldingBonus(note);
                    continue;

                default:
                    break;
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

        mNoteHeld = pNote;
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
                Instantiate(excellentPrefab);
            }

            holdCooldownTimer += Time.deltaTime;

            mNoteHeld.holdTimer += Time.deltaTime;
            if (mNoteHeld.holdTimer >= mNoteHeld.timeToHold) mNoteHeld = null;
        }
    }

    private void StrikeNote(Note note)
    {
        note.isStroked = true;

        bool isInBadTiming = note.transform.position.y >= this.transform.position.y + okTimingOffset;

        if (isInBadTiming)
        {
            mPlayerRef.GivePoints(StrokeTiming.Stroke_bad);
            return;
        }

        bool isInOkTiming = note.transform.position.y >= this.transform.position.y + excellentTimingOffset;

        if (isInOkTiming)
        {
            mPlayerRef.GivePoints(StrokeTiming.Stroke_ok);
            Instantiate(okPrefab);
            return;
        }

        bool isInExcellentTiming = note.transform.position.y >= this.transform.position.y;

        if (isInExcellentTiming)
        {
            mPlayerRef.GivePoints(StrokeTiming.Stroke_excellent);
            Instantiate(excellentPrefab);
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
}
