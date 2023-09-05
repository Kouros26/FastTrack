using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StrokingArea : MonoBehaviour
{
    [Header("Stroke Timings")]
    [SerializeField] private float badTimingOffset = 0.5f;
    [SerializeField] private float okTimingOffset = 0.3f;
    [SerializeField] private float excellentTimingOffset = 0.1f;

    

    //TODO : ActionPerformed():
    //If note is close :
    //- check the timing is bad, ok, or excellent (ajust threshold as parameters)
    //- Despawn/Hold the note based on type

    PlayerInput playerinput;

    [Header("Inputs")]
    [SerializeField]
    [Tooltip("The name of the action this stroking area is looking for. (see player input actions).")]
    private string ActionName = "";

    private InputAction action;
    private RugTrack mTrack;
    Player mPlayerRef;

    // Start is called before the first frame update
    void Start()
    {
        mPlayerRef = mTrack.GetRug().GetPlayer();

        action = mPlayerRef.GetComponent<PlayerInput>().actions[ActionName];
        action.performed += ActionPerformed;
    }

    public void ActionPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        foreach(Note note in mTrack.GetNotesOnTrack())
        {
            if (note == null) continue;

            //If under this it's either in a stroke zone or missed
            bool isInRangeOfZone = note.transform.position.y <= this.transform.position.y + badTimingOffset;

            if(!isInRangeOfZone) 
                continue;
            else
                StrokeNote(note);
        }
    }

    private void StrokeNote(Note note)
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
            return;
        }

        bool isInExcellentTiming = note.transform.position.y >= this.transform.position.y;
        
        if (isInExcellentTiming) 
        { 
            mPlayerRef.GivePoints(StrokeTiming.Stroke_excellent);
            return;
        }

        Debug.Log("Stroking a missed Note (nothing hapenned).");

        //- check the timing is bad, ok, or excellent (ajust threshold as parameters)
        //- Despawn/Hold the note based on type
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
}
