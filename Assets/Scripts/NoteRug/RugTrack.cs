using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RugTrack : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Stroking area (See the green part).")]
    private GameObject StrokingArea;

    [SerializeField]
    [Tooltip("Note Spawn Point (See the red part).")]
    private GameObject NoteSpawnPoint;

    [SerializeField]
    [Tooltip("Prefab of the note to spawn")]
    private Object NotePrefab;

    [Tooltip("List of all the notes on the track")]
    private List<Note> NotesOnTrack = new List<Note>();

    public void SpawnNote(Note pNote)
    {
        if (NoteSpawnPoint == null)
        {
            Debug.LogError("Track : " + this.name + " : Failed to spawn note, No Spawn Point.");
            return;
        }

        if (NotePrefab != null)
        {
            Object spawnedNote = Instantiate(NotePrefab, NoteSpawnPoint.transform.position, NoteSpawnPoint.transform.rotation);
       
            Note spawnedComponent = spawnedNote.GetComponent<Note>();
       
            spawnedComponent.mRugTrack  = pNote.mRugTrack;
            spawnedComponent.mType      = pNote.mType;
            spawnedComponent.noteSpeed = pNote.noteSpeed; ///ndom.Range(0, 5);

            NotesOnTrack.Add(spawnedComponent);
        }
        else
        {
            Debug.LogError("Track : " + this.name + " : Failed to spawn note, prefab not set.");
            return;
        }
    }

    private void Start()
    {
        SpawnNote(new Note());
    }

    public void Update()
    {
        foreach (var note in NotesOnTrack) 
        {
            if(note != null)
            {
                note.transform.position = Vector3.MoveTowards(note.transform.position, StrokingArea.transform.position, Time.deltaTime * note.noteSpeed);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (NoteSpawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one / 2);
        }

        if (StrokingArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(StrokingArea.transform.position, Vector3.one / 2);
        }
    }
}
