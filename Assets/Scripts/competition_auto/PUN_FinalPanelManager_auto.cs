using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PUN_FinalPanelManager_auto : MonoBehaviourPun
{
    public int MinuteCount;
    public int SecondCount;
    public float MilliCount;
    public string MilliDisplayauto;

    public GameObject MinuteDisplay;
    public GameObject SecondDisplay;
    public GameObject MilliDisplay;


    void Update()
    {
        if (SecondCount <= 9)
            SecondDisplay.GetComponent<Text>().text = "0" + SecondCount + ".";
        else
            SecondDisplay.GetComponent<Text>().text = "" + SecondCount + ".";


        if (MinuteCount <= 9)
            MinuteDisplay.GetComponent<Text>().text = "0" + MinuteCount + ":";
        else
            MinuteDisplay.GetComponent<Text>().text = "" + MinuteCount + ":";


        MilliDisplayauto = MilliCount.ToString("F0");
        MilliDisplay.GetComponent<Text>().text = "" + MilliDisplayauto;
    }
}
