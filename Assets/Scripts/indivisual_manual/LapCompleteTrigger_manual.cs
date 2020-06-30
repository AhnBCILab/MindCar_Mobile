using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LapCompleteTrigger_manual : MonoBehaviour
{

    public GameObject LapCompleteTrig;
    public GameObject HalfLapTrig;

    public GameObject MinuteDisplay;
    public GameObject SecondDisplay;
    public GameObject MilliDisplay;

    public GameObject LapTimer;

    //UIVA_Client theClient;

    public GameObject FinishPanel;
    public Text NameText;
    public AudioSource CarDefault;
    public GameObject CarControls;
    
    //int buttonIndexNum = 3;   // End condition is 3.
    SportCar_Controller_manual TempCar; // = GameObject.Find("SportCar").GetComponent<SportCar_Controller_auto>();
    DataController DC;

    void OnTriggerEnter()
    {
        TempCar = GameObject.Find("SportCar").GetComponent<SportCar_Controller_manual>();
        DC = GameObject.Find("DataController").GetComponent<DataController>();

        CarDefault.Pause();
        NameText.GetComponent<Text>().enabled = false;
        FinishPanel.SetActive(true);

        // test
        //CarControls.SetActive(false);
        //TempCar.StopCar();
        //GameObject.Find("SportCar").GetComponent<SportCar_Controller_auto>().Finish = true;



        FinalPanelManager_auto.MinuteCount = LapTimeManager_auto.MinuteCount;
        FinalPanelManager_auto.SecondCount = LapTimeManager_auto.SecondCount;
        FinalPanelManager_auto.MilliCount = LapTimeManager_auto.MilliCount;

        //FinishPanel.SetActive(true);
        LapTimer.SetActive(false);

        LapTimeManager_auto.MinuteCount = 0;
        LapTimeManager_auto.SecondCount = 0;
        LapTimeManager_auto.MilliCount = 0;

        HalfLapTrig.SetActive(true);
        LapCompleteTrig.SetActive(false);
        //FinishPanel.SetActive(true);
        //CarControls.SetActive(false);

        TempCar.StopCar();
        //Debug.Log("TempCar.Finish:" + TempCar.Finish);
    }

}
