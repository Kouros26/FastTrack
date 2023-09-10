using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using FMODUnity;

public class VictoryScreen : MonoBehaviour
{
    public ScriptablePoints mPointScriptable;

    [Header("Ojbects :")]

    [SerializeField]
    private GameObject mStar1;
    [SerializeField]
    private GameObject mStar2;
    [SerializeField]
    private GameObject mStar3;
    [Space(5)]
    [SerializeField]
    [Tooltip("Time, in seconds, of the star animation.")]
    private float mAnimDuration = 1.0f;

    [Header("Texts :")]
    [SerializeField]
    [Tooltip("Will display scores required to unlock corresponding star.")]
    private TMP_Text mText1;
    [SerializeField]
    [Tooltip("Will display scores required to unlock corresponding star.")]
    private TMP_Text mText2;
    [SerializeField]
    [Tooltip("Will display scores required to unlock corresponding star.")]
    private TMP_Text mText3;
    [Space(5)]
    [SerializeField]
    [Tooltip("Will display the score of the band.")]
    private TMP_Text mBandText;

    [Header("Points thresholds :")]

    [SerializeField]
    private int mPointsStar1 = 100000;
    [SerializeField]
    private int mPointsStar2 = 200000;
    [SerializeField]
    private int mPointsStar3 = 1000000;

    [Header("Sounds")]

    [SerializeField]
    [Tooltip("Sound that will be played when star animation is over (aka unlock sound).")]
    private StudioEventEmitter mEventStar1;
    [SerializeField]
    [Tooltip("Sound that will be played when star animation is over (aka unlock sound).")]
    private StudioEventEmitter mEventStar2;
    [SerializeField]
    [Tooltip("Sound that will be played when star animation is over (aka unlock sound).")]
    private StudioEventEmitter mEventStar3;



    public void Start()
    {
        SetUITexts();

        if (mPointScriptable.mScore >= mPointsStar1)
            StartCoroutine(StarAnimation(mStar1, mEventStar1, 0));

        if (mPointScriptable.mScore >= mPointsStar2)
            StartCoroutine(StarAnimation(mStar2, mEventStar2, 0.5f));

        if (mPointScriptable.mScore >= mPointsStar3)
            StartCoroutine(StarAnimation(mStar3, mEventStar3, 1.5f));
    }

    private void SetUITexts()
    {
        mBandText.text = "Score : " + mPointScriptable.mScore;

        mText1.text = mPointsStar1.ToString();
        mText2.text = mPointsStar2.ToString();
        mText3.text = mPointsStar3.ToString();
    }

    //I think it should be done with an animator instead.
    IEnumerator StarAnimation(GameObject pStartObject, StudioEventEmitter pAnimSound, float pAnimDelay)
    {
        if (pStartObject == null) yield break;

        yield return new WaitForSeconds(pAnimDelay);

        pStartObject.SetActive(true);

        float timer = 0;

        Vector3 baseScale = pStartObject.transform.localScale;

        pStartObject.transform.localScale = Vector3.zero;

        while (pStartObject.transform.localScale != baseScale)
        {
            timer += Time.deltaTime;
            pStartObject.transform.localScale = Vector3.Lerp(Vector3.zero, baseScale, (timer/mAnimDuration));

            yield return new WaitForEndOfFrame();
        }
        
        pAnimSound.Play();
    }
}
