using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] Slider soundSlider;

    [SerializeField] static float masterVolume = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = masterVolume;
        soundSlider.value = masterVolume;
    }

    public void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        masterVolume = volume;
    }
}
