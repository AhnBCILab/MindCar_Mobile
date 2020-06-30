using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PUN_LapCompleteTrigger_auto : MonoBehaviourPun
{
    public GameObject LapCompleteTrig;
    public GameObject HalfLapTrig;

    public GameObject MinuteDisplay;
    public GameObject SecondDisplay;
    public GameObject MilliDisplay;

    public GameObject LapTimer;
    public GameObject FinalPanelManager;
    public GameObject FinishPanel;
    public AudioSource CarDefault;
    public Text NameText;


    void OnTriggerEnter(Collider collider)
    {
        if ((PhotonNetwork.IsMasterClient && collider.tag.Equals("Master"))
            || (!PhotonNetwork.IsMasterClient && collider.tag.Equals("Client")))
        {
            LapTimer.SetActive(false);
            StartManager.StartBool = false;
            CarDefault.Pause();
            NameText.GetComponent<Text>().enabled = false;

            FinalPanelManager.GetComponent<PUN_FinalPanelManager_auto>().MinuteCount = LapTimer.GetComponent<PUN_LapTimeManager_auto>().MinuteCount;
            FinalPanelManager.GetComponent<PUN_FinalPanelManager_auto>().SecondCount = LapTimer.GetComponent<PUN_LapTimeManager_auto>().SecondCount;
            FinalPanelManager.GetComponent<PUN_FinalPanelManager_auto>().MilliCount = LapTimer.GetComponent<PUN_LapTimeManager_auto>().MilliCount;

            LapTimer.GetComponent<PUN_LapTimeManager_auto>().MinuteCount = 0;
            LapTimer.GetComponent<PUN_LapTimeManager_auto>().SecondCount = 0;
            LapTimer.GetComponent<PUN_LapTimeManager_auto>().MilliCount = 0;

            HalfLapTrig.SetActive(true);
            LapCompleteTrig.SetActive(false);
            FinishPanel.SetActive(true);
        }
    }
}
