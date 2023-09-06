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
    [SerializeField] [Tooltip("Add all rugs controlled by players.")] private List<NoteRug> rugs;

    [Header("Song")]
    [SerializeField]
    private EventReference SongEvent;

    private StudioEventEmitter mStudioEventEmitter;

    private void Awake()
    {
        InitManager();
    }
    private void Update()
    {
        UpdateNotesFromSong();
    }

    private void UpdateNotesFromSong()
    {
        if (mStudioEventEmitter.IsPlaying())
        {
            EventInstance SongEventInstannce = mStudioEventEmitter.EventInstance;

            //That's terrible - sorry T_T
            float noteSpeed;
            SongEventInstannce.getParameterByName("NoteSpeed", out noteSpeed);

            Note newNote;

            newNote = MakeNoteFromParam("Guitar1", 0);
            if (newNote != null) { rugs[0].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Guitar2", 1);
            if (newNote != null) { rugs[0].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Guitar3", 2);
            if (newNote != null) { rugs[0].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Bass1", 0);
            if (newNote != null) { rugs[1].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Bass2", 0);
            if (newNote != null) { rugs[1].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Bass3", 0);
            if (newNote != null) { rugs[1].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Drum1", 0);
            if (newNote != null) { rugs[2].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Drum2", 0);
            if (newNote != null) { rugs[2].ProcessNoteSignal(newNote); }

            newNote = MakeNoteFromParam("Drum3", 0);
            if (newNote != null) { rugs[2].ProcessNoteSignal(newNote); }
        }

    }

    private Note MakeNoteFromParam(string paramName, int rugTrack)
    {
        EventInstance SongEventInstannce = mStudioEventEmitter.EventInstance;

        float noteSpeed;
        SongEventInstannce.getParameterByName("NoteSpeed", out noteSpeed);

        float paramValue;
        SongEventInstannce.getParameterByName(paramName, out paramValue);
        SongEventInstannce.setParameterByName(paramName, 0); //optional

        if (paramValue != 0)
        {
            Note note = new Note();

            note.mType      = MathF.Round(paramValue) == 1 ? NoteType.Note_Stroke : NoteType.Note_Hold;
            note.noteSpeed  = noteSpeed;
            note.mRugTrack  = rugTrack;
            return note;
        }else
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
}
