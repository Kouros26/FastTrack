using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType
{
    Note_Stroke = 1,
    Note_Hold = 2,
    
    Note_Null = 0   
}

public enum StrokeTiming
{
    Stroke_bad,
    Stroke_ok,
    Stroke_excellent,
    Stroke_null
}

//Couldn't this be just a struct ?
public class Note : MonoBehaviour
{
    public NoteType mType = NoteType.Note_Null;
    public float    noteSpeed =  1;     //Time, in second, to reach the end of the track. Aka offset.
    public int      mRugTrack = -1;     //track id on the rug, from left to right, starting at 0, -1 is invalid;
    public bool     isStroked = false;

    public float mStrokeAreaTime = -1;     //Timestamp when note should be in stroke area.
    public float mSpawnTime = -1;          //Time note spawned
    public float  mLerpTimer = 0;           //Internal lerp value.
    public void Spawned()
    {
        this.mSpawnTime         = Time.time;
        this.mLerpTimer         = mSpawnTime;
        this.mStrokeAreaTime    = mSpawnTime + noteSpeed;
    }
}
