using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupLife : MonoBehaviour
{
    [SerializeField] Slider lifeSlider;
    [SerializeField] float lifeDecay = 0.02f;
    //[SerializeField] float mistakesDamage = 0.1f;
    //Combos aren't implemented yet

    float life = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        life -= lifeDecay * Time.deltaTime;

        lifeSlider.value = life;
    }
}
