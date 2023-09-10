using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;




[RequireComponent(typeof(StudioEventEmitter))]
public class SoundSlider : MonoBehaviour
{
    [SerializeField] Slider soundSlider;

    [SerializeField] static float masterVolume = 1.0f;
   
    [Header("Sound")]
    
    [SerializeField]
    [Tooltip("Sound that will be played when user is changing volume.")]
    StudioEventEmitter mTestSound;


    private StudioListener mListener;

    // Start is called before the first frame update
    void Start()
    {
        mListener = FindAnyObjectByType<StudioListener>();

        AudioListener.volume = masterVolume;
        soundSlider.value = masterVolume;

        if (mTestSound == null)
            Debug.LogError("Sound Slider :" + this.name + " : mTestSound is null !");
    }

    public void OnVolumeChanged(float volume)
    {
        //mListener. = volume;
        masterVolume = volume;


        if (mTestSound.IsPlaying())
            mTestSound.Stop();

        mTestSound.Play();
    }
}
