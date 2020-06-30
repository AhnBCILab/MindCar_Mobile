using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
//Added for file IO & Thread
using System.IO;

public class TurncountTrigger_auto : MonoBehaviour
{

    public GameObject HalfLapTrig;
    public GameObject TurncountTrig;
    public GameObject FinishText;
    public float time_attack;
    public float time_current;
    int buttonIndexNum = 2;   //  n/laps
    TimeStampManage TM;
    string path = "default";

    private void Start()
    {
        TM = GameObject.Find("TimeStampManage").GetComponent<TimeStampManage>();
        path = Application.persistentDataPath;
    }

    void OnTriggerEnter()
    {
        TurncountTrig.SetActive(false);
        TurnManager_auto.Turncount++;
        TM.SaveTime();
        TM.SaveEvent(TurnManager_auto.Turncount + 3);
        //Directory.CreateDirectory(path + "/TurnOffTest_" + (TurnManager_auto.Turncount+3));


        HalfLapTrig.SetActive(true);


    }
}
