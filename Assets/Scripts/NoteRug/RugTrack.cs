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

    [Tooltip("List of all the notes on the track")]
    private List<Note> NotesOnTrack = new List<Note>();

    private NoteRug mNoteRug;

    public void Awake()
    {
        if (StrokingArea != null)
            StrokingArea.SetTrack(this);
    }

    public void SetControllingPLayer(Player pPlayer)
    {
        StrokingArea.SetControllingPLayer(pPlayer);
    }

    public void SpawnNote(Note pNote)
    {
        if (NoteSpawnPoint == null)
        {
            Debug.LogError("Track : " + this.name + " : Failed to spawn note, No Spawn Point.");
            return;
        }

        //TODO : Might have to change to change the prefab for different notes types.

        Object prefabToSpawn = null;

        switch (pNote.mType)
        {
            case NoteType.Note_Stroke:
                if (NotePrefab == null) { Debug.Log("NotePrefab not set !"); } else { prefabToSpawn = NotePrefab; }
                break;

            case NoteType.Note_Hold:
                if (NoteHoldPrefab == null) { Debug.Log("NoteHoldPrefab not set !"); } else { prefabToSpawn = NoteHoldPrefab; }
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

        NotesOnTrack.Add(spawnedComponent);
    }

    public void Update()
    {
        if (!ShouldUpdateScrolling()) return;

        List<Note> notesToDestroy = new List<Note>();

        foreach (var note in NotesOnTrack)
        {
            if (note == null)
                continue;

            if (note.isStroked)
            {
                notesToDestroy.Add(note);
                continue;
            }

            note.mLerpTimer += Time.deltaTime;
            float t = note.mLerpTimer / note.mStrokeAreaTime;

            if (t > -5)
                note.transform.position = Vector3.LerpUnclamped(NoteSpawnPoint.transform.position, StrokingArea.transform.position, -t);

            else
            {
                
                notesToDestroy.Add(note);
            }
                
        }

        foreach (Note note in notesToDestroy)
        {
            DestroyNote(note);
        }

        notesToDestroy.Clear();
    }

    private void DestroyNote(Note note)
    {
        NotesOnTrack.Remove(note);
        Destroy(note.gameObject);
    }

    private bool ShouldUpdateScrolling()
    {
        //TODO : Stop scrolling when player disconnect/When game pause  
        return true;
    }

    private void OnDrawGizmos()
    {
        if (NoteSpawnPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one / 2);
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
