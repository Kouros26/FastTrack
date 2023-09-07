using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoteRug : MonoBehaviour
{
    [Tooltip("Assigned player controlling the note rug.")]
    private Player mPlayer;

    [Header("Tracks")]
    [SerializeField]
    [Tooltip("List of all tracks spawnpoints.")]
    private List<RugTrack> tracks;

    private List<Note> mNotesBuffer = new List<Note>(); //Buffer of all the notes to be spawn this frame

    private RugsManager mManager;

    private void Awake()
    {
        foreach (var track in tracks)
        {
            if (track == null) continue;

            track.SetRug(this);
        }

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
}
