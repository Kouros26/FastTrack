using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    public ScriptablePoints mPointScriptable;

    [Header("Ojbects :")]
    public GameObject mStar1;
    public GameObject mStar2;
    public GameObject mStar3;

    [Header("Points thresholds :")]
    public int mPointsStar1 = 100000;
    public int mPointsStar2 = 200000;
    public int mPointsStar3 = 1000000;

    public void Start()
    {
        if (mPointScriptable.mScore >= mPointsStar1)
            mStar1.SetActive(true);

        if (mPointScriptable.mScore >= mPointsStar2)
            mStar2.SetActive(true);

        if (mPointScriptable.mScore >= mPointsStar3)
            mStar2.SetActive(true);
    }
}
