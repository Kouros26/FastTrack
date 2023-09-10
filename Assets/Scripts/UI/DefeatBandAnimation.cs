using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//There is probably a easier way to do that than in script ?

[RequireComponent(typeof(Image))]
public class DefeatBandAnimation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the image component where the animation will be.")]
    private Image mUIImage;
    
    [SerializeField]
    [Tooltip("List of all the sprites of the animation. Will be cycled by the script.")]
    private List<Sprite> mSprites;


    [SerializeField]
    [Tooltip("Time for a full cycle of the animation.")]
    private float mAnimTime = 0.5f;

    private void Start()
    {
        if(mSprites.Count > 0)
            StartCoroutine("AnimationRoutine");
    }   

    private void OnDestroy()
    {
        StopCoroutine("AnimationRoutine");
    }

    IEnumerator AnimationRoutine()
    {
        float spriteTime = mAnimTime / mSprites.Count;
        float timer = 0;

        int indexAnim = 0;

        yield return new WaitForEndOfFrame();

        while (true) 
        {
            timer += Time.deltaTime;

            if (timer < spriteTime)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }

            timer = 0;
            indexAnim++;

            if (indexAnim >= mSprites.Count)
                indexAnim = 0;

            mUIImage.sprite = mSprites[indexAnim];

            yield return new WaitForEndOfFrame();
        }
    }
}