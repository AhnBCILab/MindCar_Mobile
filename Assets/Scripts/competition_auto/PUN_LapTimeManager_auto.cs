using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PUN_LapTimeManager_auto : MonoBehaviourPun
{
    public int lap_complete = 0;
    public int MinuteCount;
    public int SecondCount;
    public float MilliCount;
    public string MilliDisplay;

    public GameObject MinuteBox;
    public GameObject SecondBox;
    public GameObject MilliBox;

    void Update()
    {
        if (!StartManager.StartBool) return;

        MilliCount += Time.deltaTime * 100;
        MilliDisplay = MilliCount.ToString("F0");

        if (MilliCount <= 10)
            MilliBox.GetComponent<Text>().text = "0" + MilliDisplay;
        else
            MilliBox.GetComponent<Text>().text = "" + MilliDisplay;


        if (MilliCount >= 100)
        {
            MilliCount = 0;
            SecondCount += 1;
        }


        if (SecondCount <= 9)
            SecondBox.GetComponent<Text>().text = "0" + SecondCount + ".";
        else
            SecondBox.GetComponent<Text>().text = "" + SecondCount + ".";


        if (SecondCount >= 60)
        {
            SecondCount = 0;
            MinuteCount += 1;
        }


        if (MinuteCount <= 9)
        {
            //MinuteBox.GetComponent<Text>().text = "0" + MinuteCount + ":";
            MinuteBox.GetComponent<Text>().text = "0" + MinuteCount + ":";
        }
        else
            MinuteBox.GetComponent<Text>().text = "" + MinuteCount + ":";
    }





}