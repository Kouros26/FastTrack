using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Slider soundSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickedPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickedQuit()
    {
        Application.Quit();
    }

    public void OnVolumeChanged()
    {
        AudioListener.volume = soundSlider.value;
    }
}
