using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapTimeManager_auto : MonoBehaviour
{
    public int lap_complete = 0;
    public static int MinuteCount;
    public static int SecondCount;
    public static float MilliCount;
    public static string MilliDisplay;

    public GameObject MinuteBox;
    public GameObject SecondBox;
    public GameObject MilliBox;

    SportCar_Controller_auto TempCar;
    DataController DC;
    

    private void Start()
    {
        TempCar = GameObject.Find("SportCar").GetComponent<SportCar_Controller_auto>();
        DC = GameObject.Find("DataController").GetComponent<DataController>();
    }

    void Update()
    {
        MilliCount += Time.deltaTime * 100;
        MilliDisplay = MilliCount.ToString("F0");

        if (MilliCount <= 10)
        {
            MilliBox.GetComponent<Text>().text = "0" + MilliDisplay;
        }
        else
        {
            MilliBox.GetComponent<Text>().text = "" + MilliDisplay;
        }


        if (MilliCount >= 100)
        {
            MilliCount = 0;
            SecondCount += 1;
        }

        if (SecondCount <= 9)
        {
            SecondBox.GetComponent<Text>().text = "0" + SecondCount + ".";
            //SecondBox.GetComponent<Text>().text = "" + TempCar.SpeedList.Count + ".";
            //SecondBox.GetComponent<Text>().text = "" + DC.Algo_Attention + ".";
        }
        else
        {
            SecondBox.GetComponent<Text>().text = "" + SecondCount + ".";
            //SecondBox.GetComponent<Text>().text = "" + TempCar.SpeedList.Count + ".";
            //SecondBox.GetComponent<Text>().text = "" + DC.Algo_Attention + ".";
        }

        if (SecondCount >= 60)
        {
            SecondCount = 0;
            MinuteCount += 1;
        }

        if (MinuteCount <= 9)
        {
            MinuteBox.GetComponent<Text>().text = "0" + MinuteCount + ":";
            //MinuteBox.GetComponent<Text>().text = "" + DC.RawList.Count + ":";
        }
        else
        {
            MinuteBox.GetComponent<Text>().text = "" + MinuteCount + ":";
        }

    }





}