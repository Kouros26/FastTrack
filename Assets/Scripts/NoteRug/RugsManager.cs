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
    [Header("Rugs :")]
    [SerializeField][Tooltip("Add all rugs controlled by players.")] private List<NoteRug> rugs;

    [Header("Song :")]
    [SerializeField]
    private EventReference SongEvent;

    [Space(10)]
    [Header("Coop Bonuses :")]
    [Space(10)]

    [SerializeField]
    [Range(0.0f, 30.0f)]
    [Tooltip("Time, in seconds, between two bonus phases.")]
    private float mBonusPhaseCooldown = 30;

    [SerializeField]
    [Range(0.0f, 30.0f)]
    [Tooltip("Time, in second, which will help generate a random offset btween 0 and mBonusPhaseRandomModifier to sligtly randomise the bonus phase.")]
    private float mBonusPhaseRandomModifier = 5;

    [SerializeField]
    [Range(0.0f, 30.0f)]
    [Tooltip("When in bonus phase, time, in seconds, between two spawn (global cooldown between tracks).")]
    private float mBonusNoteSpawnCooldown = 5;

    [SerializeField]
    [Range(0.0f, 30.0f)]
    [Tooltip("Random Time, in second, between 0 and mBonusNoteSpawnRandomModifier that will be added to mBonusNoteSpawnCooldown.")]
    private float mBonusNoteSpawnRandomModifier = 2;

    [Range(0.0f, 30.0f)]
    [Tooltip("Time, in second, for to reach the end of the track. Aka offset.")]
    public float mBonusNoteSpeed = 5;

    private float mBonusNoteSpawnTimer = 0;
    private float mRealBonusNoteSpawnCooldown = 0;

    private float mRealBonusPhaseCooldown = 0;

    private float mBonusPhaseTimer = 0;
    private bool isInBonusPhase = false;
    private List<bool> bonusHelds = new List<bool>();

    private Note noteFromSong = null; //"Buffer" Used for note generation.
    private StudioEventEmitter mStudioEventEmitter;
    private int playerCount = 0;

    private void Awake()
    {
        InitManager();
        noteFromSong = this.AddComponent<Note>();
    }

    private void Update()
    {
        UpdateBonusPhase();
    }

    private void FixedUpdate()
    {
        UpdateNotesFromSong(); //from fmod
    }

    private void UpdateBonusPhase()
    {
        if (isInBonusPhase)
        {
            UpdateBonusSpawn();
        }

        mBonusPhaseTimer += Time.deltaTime;

        if (mBonusPhaseTimer > mRealBonusPhaseCooldown)
        {
            StartBonusPhaase();
        }
    }

    private void UpdateBonusSpawn()
    {
        mBonusNoteSpawnTimer += Time.deltaTime;

        if (mBonusNoteSpawnTimer <= mRealBonusNoteSpawnCooldown)
            return;
        
        mBonusNoteSpawnTimer = 0;
        mRealBonusNoteSpawnCooldown = mBonusNoteSpawnCooldown + UnityEngine.Random.Range(0.0f, mBonusNoteSpawnRandomModifier);

        List<NoteRug> rugsCopy = new List<NoteRug>(rugs);

        //Spawn a bonus note on a free track.

        for (int i = 0; i < rugsCopy.Count; i++)
        {
            NoteRug rug = rugsCopy[UnityEngine.Random.Range(0, rugsCopy.Count)];

            if (rug.CanSpawnBonusNote())
            {
                rug.SpawnBonusNote();
                return;
            }
            else
            {
                rugsCopy.Remove(rug);
            }
        }
    }

    private void StartBonusPhaase()
    {
        isInBonusPhase = true;
        mBonusPhaseTimer = 0;
        Debug.Log("Start of Bonus Phase");
    }

    public void StopBonusPhase()
    {
        mBonusPhaseTimer = 0;
        isInBonusPhase = false;
        mRealBonusPhaseCooldown = mBonusPhaseCooldown + UnityEngine.Random.Range(0.0f, mBonusPhaseRandomModifier);
        Debug.Log("Stopped Bonus Phase");
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

            noteFromSong.mType = MathF.Round(paramValue) == 10 ? NoteType.Note_Stroke : NoteType.Note_Hold;
            if (noteFromSong.mType == NoteType.Note_Hold) { noteFromSong.timeToHold = paramValue; }
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

        //Play sound
        mStudioEventEmitter = this.AddComponent<StudioEventEmitter>();
        mStudioEventEmitter.EventReference = this.SongEvent;
        mStudioEventEmitter.Play();

        //Bonus Phase
        mRealBonusNoteSpawnCooldown = mBonusNoteSpawnCooldown + UnityEngine.Random.Range(0.0f, mBonusNoteSpawnRandomModifier);
        mRealBonusPhaseCooldown = mBonusPhaseCooldown + UnityEngine.Random.Range(0.0f, mBonusPhaseRandomModifier);
    }

    //Assign an empty rug to a player.
    public void AssignRug(Player pPlayer)
    {
        foreach (var rug in rugs)
        {
            if (rug.GetPlayer() == null)
            {
                rug.SetControllingPlayer(pPlayer);
                playerCount++;
                return;
            }
            else
            {
                Debug.LogWarning("Extra player spawned. Note more rug for them.");
            }
        }

    }
}
