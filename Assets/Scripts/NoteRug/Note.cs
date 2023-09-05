using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType
{
    Note_Stroke = 1,
    Note_Hold = 2,
    
    Note_Null = 0   
}
public class Note : MonoBehaviour
{
   public NoteType  mType = NoteType.Note_Null;
   public float     noteSpeed = 1;      //
   public int       mRugTrack = 0;      //Columuns on the rug, from left to right, starting at 0;
}
