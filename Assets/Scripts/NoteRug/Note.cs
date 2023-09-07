using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType
{
    //Coming from fmod.
    Note_Stroke = 1,
    Note_Hold = 2,
    
    Note_BonusShard,

    Note_Null = -1  
}

public enum StrokeTiming
{
    Stroke_bad,
    Stroke_ok,
    Stroke_excellent,
    Stroke_holding,
    Stroke_missed,
    Stroke_null = -1
}

//Couldn't this be just a struct ?
public class Note : MonoBehaviour
{
    public NoteType mType = NoteType.Note_Null;
    
    public float    noteSpeed =  1;     //Time, in second, for to reach the end of the track. Aka offset.
    public int      mRugTrack = -1;     //Track id on the rug, from left to right, starting at 0, -1 is invalid;
    public float    timeToHold = -1;    //Time, in second, the player can hold the note to gain more points.
    public float    holdTimer = 0;      //Internal hold value.
    public bool     isStroked = false;


    public float mStrokeAreaTime = -1;     //Timestamp when note should be in stroke area.
    public float mSpawnTime = -1;          //Time note spawned
    public float mLerpTimer = 0;           //Internal lerp value.
    
    public void Spawned()
    {
        this.mSpawnTime = Time.time;
        this.mLerpTimer = mSpawnTime;
        this.mStrokeAreaTime = mSpawnTime + noteSpeed;
    }


    public void ResetComponent()
    {
        mType = NoteType.Note_Null;
        noteSpeed = 1;
        mRugTrack = -1;
        timeToHold = -1;
        holdTimer = 0;
        isStroked = false;

        mStrokeAreaTime = -1;
        mSpawnTime = -1;
        mLerpTimer = 0;
    }
}
