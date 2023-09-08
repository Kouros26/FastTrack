using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RugTrack : MonoBehaviour
{
    [Header("Points :")]

    [SerializeField]
    [Tooltip("Stroking area (See the green part).")]
    private StrokingArea StrokingArea;

    [SerializeField]
    [Tooltip("Note Spawn Point (See the red part).")]
    private GameObject NoteSpawnPoint;

    [Space(10)]
    [Header("Prefabs : ")]

    [SerializeField]
    [Tooltip("Prefab to spawn when a basic note is appearing.")]
    private Object NotePrefab;

    [SerializeField]
    [Tooltip("Prefab to spawn when a note to hold is appearing.")]
    private Object NoteHoldPrefab;

    [SerializeField]
    [Tooltip("Prefab to spawn when a bonus note is appearing.")]
    private Object BonusNotePrefab;

    [Tooltip("List of all the notes on the track")]
    private List<Note> NotesOnTrack = new List<Note>();

    private NoteRug mNoteRug;

    private Player mPlayer = null;

    public void Awake()
    {
        if (StrokingArea != null)
            StrokingArea.SetTrack(this);
    }

    public void SetControllingPLayer(Player pPlayer)
    {
        StrokingArea.SetControllingPLayer(pPlayer);
        mPlayer = pPlayer;
    }

    public void SpawnNote(Note pNote)
    {
        if (NoteSpawnPoint == null)
        {
            Debug.LogError("Track : " + this.name + " : Failed to spawn note, No Spawn Point.");
            return;
        }

        Object prefabToSpawn = null;

        switch (pNote.mType)
        {
            case NoteType.Note_Stroke:
                if (NotePrefab == null) { Debug.Log("NotePrefab not set !"); } else { prefabToSpawn = NotePrefab; }
                break;

            case NoteType.Note_Hold:
                if (NoteHoldPrefab == null) { Debug.Log("NoteHoldPrefab not set !"); } else { prefabToSpawn = NoteHoldPrefab; }
                break;

            case NoteType.Note_BonusShard:
                if (BonusNotePrefab == null) { Debug.Log("BonusNotePrefab not set !"); } else { prefabToSpawn = BonusNotePrefab; }
                break;

            default:
                break;
        }

        Object spawnedNote = Instantiate(prefabToSpawn, NoteSpawnPoint.transform.position, NoteSpawnPoint.transform.rotation);

        Note spawnedComponent = spawnedNote.GetComponent<Note>();

        if (spawnedComponent == null) spawnedNote = spawnedNote.AddComponent<Note>();

        spawnedComponent.mType = pNote.mType;
        spawnedComponent.mRugTrack = pNote.mRugTrack;
        spawnedComponent.noteSpeed = pNote.noteSpeed;
        spawnedComponent.timeToHold = pNote.timeToHold;

        //Hold Note - tail scaling
        if(spawnedComponent.mType == NoteType.Note_Hold)
        {
            NoteTail tail = spawnedComponent.GetComponentInChildren<NoteTail>();

            float totalDistance = (NoteSpawnPoint.transform.position - StrokingArea.transform.position).magnitude;

            float tailHeight = (totalDistance * spawnedComponent.timeToHold) / spawnedComponent.noteSpeed;

            Vector3 tailScale = new Vector3(tail.transform.localScale.x, tail.transform.localScale.y, tailHeight / 100); //Unity units to "scale units"
            float tailYOffset = tailScale.z * 5;
            Vector3 tailPosition = new Vector3(tail.transform.localPosition.x, tailYOffset, tail.transform.localPosition.z);

            tail.transform.localScale = tailScale;
            tail.transform.localPosition = tailPosition;
        }

        NotesOnTrack.Add(spawnedComponent);
    }

    public void Update()
    {
        //Terrible code.

        List<Note> notesToDestroy = new List<Note>();

        foreach (var note in NotesOnTrack)
        {
            if (note == null || (note.mType == NoteType.Note_BonusShard && StrokingArea.IsHoldingBonus()))
                continue;

            if (note.isStroked)
            {
                notesToDestroy.Add(note);
                continue;
            }

            note.mLerpTimer += Time.deltaTime / note.noteSpeed;
            float t = note.mLerpTimer / note.mStrokeAreaTime;

            //Just past the line
            if (t < -1)
                NoteMissed(note);

            //Hide note when a little under the bar
            if (t < -1.2)
                note.gameObject.transform.localScale = Vector3.zero;

            //Destroy if needed, update otherwise
            if (t > -5)
                note.transform.position = Vector3.LerpUnclamped(NoteSpawnPoint.transform.position, StrokingArea.transform.position, -t);
            else
                notesToDestroy.Add(note);
                
        }

        foreach (Note note in notesToDestroy)
        {
            DestroyNote(note);
        }

        notesToDestroy.Clear();
    }

    private void NoteMissed(Note pNote)
    {
        if (!pNote.isMissed && pNote.mType != NoteType.Note_BonusShard)
        {
            pNote.isMissed = true; //To be sure this is called once per note.
            GetRug().NoteIsMissed(); 
        }
    }

    private void DestroyNote(Note note)
    {
        NotesOnTrack.Remove(note);
        Destroy(note.gameObject);
    }

    private bool ShouldUpdateScrolling()
    {
        return mPlayer != null;
    }

    private void OnDrawGizmos()
    {
        if (NoteSpawnPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(NoteSpawnPoint.transform.position, Vector3.one / 2);
    }

    public void SetRug(NoteRug pNoteRug)
    {
        mNoteRug = pNoteRug;
    }

    public NoteRug GetRug()
    {
        return mNoteRug;
    }

    public ReadOnlyCollection<Note> GetNotesOnTrack()
    {
        return NotesOnTrack.AsReadOnly();
    }
}
