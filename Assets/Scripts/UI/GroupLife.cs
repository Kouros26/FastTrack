using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupLife : MonoBehaviour
{
    [SerializeField] int life;
    [SerializeField] int maxLife;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnLifeChange(int deltaLife)
    {
        if (life + deltaLife > maxLife)
            life = maxLife;

        life += deltaLife;
    }
}
