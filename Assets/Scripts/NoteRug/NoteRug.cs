using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRug : MonoBehaviour
{
    //TODO : Player stroking the notes (input-timing-type-points)
    //TODO : Scroll notes


    [Tooltip("Assigned player controlling the note rug.")] 
    private Player mPlayer;

    [SerializeField]
    [Tooltip("List of all tracks spawnpoints.")]
    private List<RugTrack> tracks;

    private List<Note> mNotesBuffer = new List<Note>(); //Buffer of all the notes to be spawn this frame
    
    private RugsManager mManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mNotesBuffer.Count > 0) 
        {
            ProcessNoteBuffer();
        }    
    }

    public void FixedUpdate()
    {

    }

    private void ProcessNoteBuffer()
    {
        foreach(Note note in mNotesBuffer)
        {
            this.SpawnNote(note);
        }

        mNotesBuffer.Clear();
    }

    private void SpawnNote(Note pNnote)
    {
        tracks[pNnote.mRugTrack].SpawnNote(pNnote);
    }

    public void SetControllingPlayer(Player pPlayer)
    {
        if (mPlayer != null)
            this.mPlayer = pPlayer;
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
}
