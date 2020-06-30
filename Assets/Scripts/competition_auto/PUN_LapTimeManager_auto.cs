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
    /*
     public GameObject LabelBox;

     public void update_laps(int i)
     {
         if (i==1)
         {
             update_1stlap();
         }
         else if(i==2)
         {
             update_2ndlap();
         }
     }
     public void update_1stlap()
     {
         LabelBox.GetComponent<Text>().text = "Lap Time" + "\n" + "1st Lap:   "
             + LapTimeManager_auto.MinuteCount + ":" + LapTimeManager_auto.SecondCount + ":" + LapTimeManager_auto.MilliCount.ToString("F0");
     }

     public void update_2ndlap()
     {
         LabelBox.GetComponent<Text>().text = LabelBox.GetComponent<Text>().text + "\n" + "2nd Lap:   "
             + LapTimeManager_auto.MinuteCount + ":" + LapTimeManager_auto.SecondCount + ":" + LapTimeManager_auto.MilliCount.ToString("F0");
     }
     */

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