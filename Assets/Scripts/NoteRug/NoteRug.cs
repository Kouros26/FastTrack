using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(StudioEventEmitter))]
public class NoteRug : MonoBehaviour
{
    [Tooltip("Assigned player controlling the note rug.")]
    private Player mPlayer;

    [Header("Tracks")]
    [SerializeField]
    [Tooltip("List of all tracks.")]
    private List<RugTrack> tracks;

    [SerializeField]
    [Tooltip("Bonus Track object. (use prefab)")]
    private RugTrack mBonusTrack = null;

    [Header("Sound Events :")]
    [SerializeField]
    [Tooltip("Sound event played when player miss a note.")]
    private StudioEventEmitter mSEventNoteMissed;

    private List<Note> mNotesBuffer = new List<Note>(); //Buffer of all the notes to be spawn this frame

    private RugsManager mManager;

    private void Awake()
    {
        foreach (var track in tracks)
        {
            if (track == null) continue;

            track.SetRug(this);
        }

        mBonusTrack.SetRug(this);   

        //StartCoroutine(Debug_ContinuousSpawn());
    }

    IEnumerator Debug_ContinuousSpawn()
    {
        Note note = new Note();

        while (true)
        {
            note.mRugTrack = Random.Range(0, 3);
            //note.mType = (NoteType)Random.Range(1,3);
            note.mType = NoteType.Note_Hold;
            note.timeToHold = 2;
            SpawnNote(note);
            yield return new WaitForSeconds(7f);
        }
    }

    ///Call this when player miss a note on a track.
    public void NoteIsMissed()
    {
        if (!mSEventNoteMissed.IsPlaying())
            mSEventNoteMissed.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (mNotesBuffer.Count > 0)
        {
            ProcessNoteBuffer();
        }
    }

    private void ProcessNoteBuffer()
    {
        foreach (Note note in mNotesBuffer)
        {
            SpawnNote(note);
        }

        mNotesBuffer.Clear();
    }

    private void SpawnNote(Note pNnote)
    {
        if (pNnote.mRugTrack < 0)
        {
            Debug.LogError("Rug : " + this.name + " : Couldn't spawn note : Invalid track id");
            return;
        }

        tracks[pNnote.mRugTrack].SpawnNote(pNnote);
    }

    public void SetControllingPlayer(Player pPlayer)
    {
        mPlayer = pPlayer;

        foreach(var track in tracks)
        {
            track.SetControllingPLayer(pPlayer);
        }

        mBonusTrack.SetControllingPLayer(pPlayer);

        Debug.Log(this.name + " : Player have been asigned !");
    }

    public void SetRugManager(RugsManager pManager)
    {
        if (pManager != null)
            mManager = pManager;
    }

    //Process Notes comming from fmod. Sent from the RugManager.
    public void ProcessNoteSignal(Note pNote)
    {
        mNotesBuffer.Add(pNote);
    }

    public Player GetPlayer()
    {
        return mPlayer;
    }

    public bool CanSpawnBonusNote()
    {
        return mBonusTrack != null && mBonusTrack.GetNotesOnTrack().Count <= 0 && mPlayer != null;
    }

    public void SpawnBonusNote()
    {
        Note bonusNote = new Note();

        bonusNote.mType = NoteType.Note_BonusShard;
        bonusNote.noteSpeed = mManager.mBonusNoteSpeed;        

        mBonusTrack.SpawnNote(bonusNote);
        Debug.Log(this.name + " : Spawning bonus note !");
    }

    internal RugsManager GetManager()
    {
        return mManager;
    }
}
