using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class RugsManager : MonoBehaviour
{
    [SerializeField] [Tooltip("Add all rugs controlled by players.")] private List<NoteRug> rugs;

    [SerializeField]
    private EventReference SongEvent; 

    private StudioEventEmitter mStudioEventEmitter;

    private void Awake()
    {
        InitManager();
    }
    private void FixedUpdate()
    {
        //Setps :
        //- Get event values, and pack it into a note.
        //- Set events at 0
        //- Broadcast all the notes to the rugs (if correct type)


        if (mStudioEventEmitter.IsPlaying())
        {
            EventInstance SongEventInstannce = mStudioEventEmitter.EventInstance;

            foreach(var currentParameter in mStudioEventEmitter.Params)
            {
                //- Get event values, and pack it into a note.
                Note SongNote = new Note();

                float parameterValue;

                SongEventInstannce.getParameterByID(currentParameter.ID, out parameterValue);

                SongNote.mType      = (NoteType)Math.Round(parameterValue);
                SongNote.mRugTrack  = (int)parameterValue;

                //- Set events at 0
                SongEventInstannce.setParameterByID(currentParameter.ID, 0);

                //- Broadcast all the notes to the rugs (if correct type)
                //if(correct rug)
                    rugs[0].ProcessNoteSignal(SongNote);
            }
        }
    }

    private void InitManager()
    {
        foreach (var rug in rugs)
        {
            rug.SetRugManager(this);
        }

        mStudioEventEmitter = new FMODUnity.StudioEventEmitter();

        mStudioEventEmitter.EventReference  = this.SongEvent;
        mStudioEventEmitter.Play();

        foreach (var currentParameter in mStudioEventEmitter.Params)
        {
            Debug.Log("Param : " + currentParameter.Name);
        }
    }
}
