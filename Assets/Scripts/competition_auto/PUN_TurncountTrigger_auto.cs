using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
//Added for file IO & Thread
using System.IO;

public class PUN_TurncountTrigger_auto : MonoBehaviourPun
{
    public GameObject HalfLapTrig;
    public GameObject TurncountTrig;
    public float time_attack;
    public float time_current;
    int buttonIndexNum = 2;   //  n/laps

    //public GameObject TM;
    public PhotonView PV;
    TimeStampManage TM;
    string path = "default";

    private void Start()
    {
        TM = GameObject.Find("TimeStampManage").GetComponent<TimeStampManage>();
        path = Application.persistentDataPath;
    }

    void OnTriggerEnter(Collider collider)
    {
        //TM.SaveTime();
        if (PhotonNetwork.IsMasterClient && collider.tag.Equals("Master"))
        {
            GameManager.playerTurncounts[0]++;
            TM.SaveEvent(GameManager.playerTurncounts[0] + 3);
            TM.SaveTime();
            //Directory.CreateDirectory(path + "/TurnOffTest_" + (GameManager.playerTurncounts[0] + 3));
            if (GameManager.playerTurncounts[0] == 3) GameManager.playerTurncounts[0] = 0;

            TurncountTrig.SetActive(false);
            HalfLapTrig.SetActive(true);
        }
        else if (!PhotonNetwork.IsMasterClient && collider.tag.Equals("Client"))
        {
            GameManager.playerTurncounts[1]++;
            TM.SaveEvent(GameManager.playerTurncounts[1] + 3);
            TM.SaveTime();
            //Directory.CreateDirectory(path + "/TurnOffTest_" + (GameManager.playerTurncounts[1] + 3));
            if (GameManager.playerTurncounts[1] == 3) GameManager.playerTurncounts[1] = 0;

            TurncountTrig.SetActive(false);
            HalfLapTrig.SetActive(true);
        }
    }
}
