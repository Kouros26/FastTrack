using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [SerializeField] Object gameUI;
    [SerializeField] Object pauseMenu;
    [SerializeField] Object optionsMenu;
    [SerializeField] Button resumeButton;
    [SerializeField] RugsManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindAnyObjectByType<RugsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPause()
    {
        manager.OnPause();
        Time.timeScale = 0.0f;

        if (!pauseMenu.GameObject().activeSelf && !optionsMenu.GameObject().activeSelf)
        {
            gameUI.GameObject().SetActive(false);
            pauseMenu.GameObject().SetActive(true);
            resumeButton.Select();

        }
    }

    public void OnResume()
    {
        manager.OnResume();
        Time.timeScale = 1.0f;
        //Pause menu gets deactivated out of the script and gameUI also gets reactivated out of the script
    }

    public void OnClickedQuit()
    {
        //Go to main menu
        SceneManager.LoadScene(0);
    }
}
