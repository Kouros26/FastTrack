using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class RugsManager : MonoBehaviour
{
    [SerializeField][Tooltip("Add all rugs controlled by players.")] private List<NoteRug> rugs;

    [Header("Song")]
    [SerializeField]
    private EventReference SongEvent;

    private StudioEventEmitter mStudioEventEmitter;
    private Note noteFromSong = null;

    private void Awake()
    {
        InitManager();
        noteFromSong = this.AddComponent<Note>();
    }

    private void FixedUpdate()
    {
        UpdateNotesFromSong();
    }

    private void UpdateNotesFromSong()
    {
        if (mStudioEventEmitter.IsPlaying())
        {
            EventInstance SongEventInstannce = mStudioEventEmitter.EventInstance;

            //

            Note newNote;

            newNote = MakeNoteFromParam("Guitar1", 0);
            if (newNote != null) { rugs[0].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Guitar2", 1);
            if (newNote != null) { rugs[0].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Guitar3", 2);
            if (newNote != null) { rugs[0].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Bass1", 0);
            if (newNote != null) { rugs[1].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Bass2", 1);
            if (newNote != null) { rugs[1].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Bass3", 2);
            if (newNote != null) { rugs[1].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Drums1", 0);
            if (newNote != null) { rugs[2].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Drums2", 1);
            if (newNote != null) { rugs[2].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Drums3", 2);
            if (newNote != null) { rugs[2].ProcessNoteSignal(newNote); }
        }

    }

    private Note MakeNoteFromParam(string paramName, int rugTrack)
    {
        EventInstance SongEventInstannce = mStudioEventEmitter.EventInstance;

        float placeHolder = 0; //What is this value for ? No docs ??
        float noteSpeed = 0;

        SongEventInstannce.getParameterByName("NoteSpeed", out placeHolder, out noteSpeed);
        //SongEventInstannce.setParameterByName("NoteSpeed", 0);

        float paramValue = 0;

        SongEventInstannce.getParameterByName(paramName, out placeHolder, out paramValue);
        SongEventInstannce.setParameterByName(paramName, 0);

        
        if (paramValue != 0)
        {
            noteFromSong.ResetComponent(); //Hack, I can't just create a new instance caus it's Monobehaviour.

            noteFromSong.mType = MathF.Round(paramValue) == 1 ? NoteType.Note_Stroke : NoteType.Note_Hold;
            if (noteFromSong.mType == NoteType.Note_Hold) { } //TODO : Check with sound design about fmod impl of that.
            noteFromSong.noteSpeed = noteSpeed;
            noteFromSong.mRugTrack = rugTrack;

            return noteFromSong;
        }
        else
        {
            return null;
        }
    }


    private void InitManager()
    {
        foreach (var rug in rugs)
        {
            rug.SetRugManager(this);
        }

        mStudioEventEmitter = this.AddComponent<StudioEventEmitter>();
        mStudioEventEmitter.EventReference = this.SongEvent;
        mStudioEventEmitter.Play();
    }

    //Assign an empty rug to a player.
    public void AssignRug(Player pPlayer)
    {
        foreach (var rug in rugs)
        {
            if (rug.GetPlayer() == null)
            {
                rug.SetControllingPlayer(pPlayer);
                return;
            }
            else
            {
                Debug.LogWarning("Extra player spawned. Note more rug for them.");
            }

        }
    }
}
