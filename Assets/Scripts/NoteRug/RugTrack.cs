using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
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
    [Tooltip("Prefab of the note to spawn")]
    private Object NotePrefab;

    [Tooltip("List of all the notes on the track")]
    private List<Note> NotesOnTrack = new List<Note>();
    
    private NoteRug mNoteRug;

    public void Awake()
    {
        if (StrokingArea != null)
            StrokingArea.SetTrack(this);
    }

    public void SpawnNote(Note pNote)
    {
        if (NoteSpawnPoint == null)
        {
            Debug.LogError("Track : " + this.name + " : Failed to spawn note, No Spawn Point.");
            return;
        }

        //TODO : Might have to change to change the prefab for different notes types.

        if (NotePrefab != null)
        {
            Object spawnedNote = Instantiate(NotePrefab, NoteSpawnPoint.transform.position, NoteSpawnPoint.transform.rotation);
       
            Note spawnedComponent = spawnedNote.GetComponent<Note>();
       
            spawnedComponent.mType      = pNote.mType;
            spawnedComponent.mRugTrack  = pNote.mRugTrack;
            spawnedComponent.noteSpeed  = pNote.noteSpeed;

            NotesOnTrack.Add(spawnedComponent);
        }
        else
        {
            Debug.LogError("Track : " + this.name + " : Failed to spawn note, prefab not set.");
            return;
        }
    }

    public void Update()
    {
        if (!ShouldUpdateNotes()) return;

        List<Note> notesToDestroy = new List<Note>();

        foreach (var note in NotesOnTrack)
        {

            if (note == null)
                continue;

            if(note.isStroked)
            {
                notesToDestroy.Add(note);
                continue;
            }

            Vector3 nextPosition = Vector3.MoveTowards(note.transform.position, StrokingArea.transform.position, Time.deltaTime * note.noteSpeed);

            if (note.transform.position != nextPosition)
                note.transform.position = nextPosition;
            else
                notesToDestroy.Add(note);
        }

        foreach(Note note in notesToDestroy)
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

    private bool ShouldUpdateNotes()
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
