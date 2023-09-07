using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessPopUp : MonoBehaviour
{
    [SerializeField] float timeBeforeDestroy = 0.5f;
    float elapsedTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime > timeBeforeDestroy)
            Destroy(gameObject);
    }
}
