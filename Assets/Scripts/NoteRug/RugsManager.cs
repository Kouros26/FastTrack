using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
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

    [SerializeField]
    [Range(0.0f, 30.0f)]
    [Tooltip("Time, in second, when all players need to release their bonus to activate the effect.")]
    private float mBonusReleaseWindow = 0.64f;

    [SerializeField]
    [Range(0.0f, 30.0f)]
    [Tooltip("Time, in second, of the bonus effect.")]
    private float mBonusDuration = 10.0f;

    private float mBonusNoteSpawnTimer = 0;
    private float mRealBonusNoteSpawnCooldown = 0;

    private float mRealBonusPhaseCooldown = 0;

    private float mBonusPhaseTimer = 0;
    private bool isInBonusPhase = false;
    private List<bool> bonusHelds = new List<bool>();

    private StudioEventEmitter mStudioEventEmitter;

    private Note noteFromSong = null;   //"Buffer" Used for note generation.
    private int playerCount = 0;

    private int bonusHeldCount = 0;     //The number of bonuses held by the players.

    private bool BonusReleaseCoroutine = false;

    public List<Player> playerList = new List<Player>();

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
        mRealBonusNoteSpawnCooldown = mBonusNoteSpawnCooldown + UnityEngine.Random.Range(0.0f, mBonusNoteSpawnRandomModifier + 1);

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
            UpdateRug(rugs[0], "Guitar");
            UpdateRug(rugs[1], "Bass");
            UpdateRug(rugs[2], "Drums");
        }
        else
        {
            FindObjectOfType<GroupLife>().MakeGameOver();
        }
    }

    private void UpdateRug(NoteRug pRug, string pInstrument)
    {
        if (pRug.GetPlayer() == null)
            return;

        Note newNote;

        newNote = MakeNoteFromParam(pInstrument + "1", 0);
        if (newNote != null) { pRug.ProcessNoteSignal(newNote); }

        newNote = MakeNoteFromParam(pInstrument + "2", 1);
        if (newNote != null) { pRug.ProcessNoteSignal(newNote); }

        newNote = MakeNoteFromParam(pInstrument + "3", 2);
        if (newNote != null) { pRug.ProcessNoteSignal(newNote); }
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

            noteFromSong.mType = MathF.Round(paramValue) == 30 ? NoteType.Note_Stroke : NoteType.Note_Hold;
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
                playerList.Add(pPlayer);
                playerCount++;
                return;
            }
            else
            {
                Debug.LogWarning("Extra player spawned. Note more rug for them.");
            }
        }

    }

    public void NewBonusIsHeld()
    {
        bonusHeldCount++;
    }

    public void BonusIsReleased()
    {
        if (!BonusReleaseCoroutine && bonusHeldCount >= playerCount)
            StartCoroutine("GroupBonusReleaseWindow");

        bonusHeldCount--;
    }

    IEnumerator GroupBonusReleaseWindow()
    {
        float groupTimer = mBonusReleaseWindow;
        BonusReleaseCoroutine = true;

        while (groupTimer > 0)
        {
            groupTimer -= Time.deltaTime;

            if (bonusHeldCount <= 0)
            {
                StartCoroutine("BonusEffect");
                BonusReleaseCoroutine = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        BonusReleaseCoroutine = false;
    }

    IEnumerator BonusEffect()
    {
        foreach (var player in playerList)
        {
            player.EnableBonus();
        }

        yield return new WaitForSeconds(mBonusDuration);

        foreach (var player in playerList)
        {
            player.DisableBonus();
        }
    }

    public void StopMusic()
    {
        //TODO : Should i do fadeout from code ?
        mStudioEventEmitter.Stop();
    }

    //Direct reference :/
    internal int GetGuitarScore()
    {
        if (playerList.Count < 1) return -1;
        return playerList[0] != null ? playerList[0].GetPoints() : -1;
    }

    internal int GetDrumScore()
    {
        if (playerList.Count < 2) return -1;
        return playerList[1] != null ? playerList[0].GetPoints() : -1;
    }

    internal int GetBassScore()
    {
        if (playerList.Count < 3) return -1;
        return playerList[2] != null ? playerList[0].GetPoints() : -1;
    }

    internal void OnPause()
    {
        mStudioEventEmitter.EventInstance.setPaused(true);
    }

    internal void OnResume()
    {
        mStudioEventEmitter.EventInstance.setPaused(false);
    }
}
