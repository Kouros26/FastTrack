using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [SerializeField] Object pauseMenu;
    [SerializeField] Object optionsMenu;
    [SerializeField] Button resumeButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPause()
    {
        Time.timeScale = 0.0f;

        if (!pauseMenu.GameObject().activeSelf && !optionsMenu.GameObject().activeSelf)
        {
            pauseMenu.GameObject().SetActive(true);
            resumeButton.Select();
        }
    }

    public void OnResume()
    {
        Time.timeScale = 1.0f;
        //Pause menu gets deactivated out of the script
    }

    public void OnClickedQuit()
    {
        Application.Quit();
    }
}
