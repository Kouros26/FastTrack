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
    
    private StudioEventEmitter mStudioEventEmitter;

    [EventRef]
    public string SongEvent;

    private void Awake()
    {
        InitManager();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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

            //foreach(parameters)
            {

                //- Get event values, and pack it into a note.
                Note SongNote = new Note();

                float parameterValue;

                SongEventInstannce.getParameterByName("", out parameterValue);
                SongNote.mType = (NoteType)Math.Round(parameterValue);


                //- Set events at 0
                SongEventInstannce.setParameterByName("", 0);

                //- Broadcast all the notes to the rugs (if correct type)
                //if(rug)
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

        mStudioEventEmitter.PlayEvent   = FMODUnity.EmitterGameEvent.ObjectStart;
        mStudioEventEmitter.Event       = this.SongEvent;          //TODO : Fix deprecated ?
    }
}
