using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupLife : MonoBehaviour
{
    [SerializeField] Slider lifeSlider;
    [SerializeField] float lifeDecay = 0.02f;

    [Tooltip("Life will be updated on the slider every x bits of life lost (life goes from 0 to 1)")]
    [SerializeField] float lifeDisplayThreshold = 0.1f;
    //[SerializeField] float mistakesDamage = 0.1f;
    //Combos aren't implemented yet

    float life = 1.0f;
    float currentLifeLost = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        currentLifeLost += lifeDecay * Time.deltaTime;
        life -= lifeDecay * Time.deltaTime;

        if (currentLifeLost >= lifeDisplayThreshold)
        {
            lifeSlider.value = life;
            currentLifeLost = 0.0f;
        }

    }
}
