using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUN_HalfPointTrigger_auto : MonoBehaviourPun
{
    public GameObject LapCompleteTrig;
    public GameObject HalfLapTrig;
    public GameObject TurncountTrig;
   
    void OnTriggerEnter(Collider collider)
    {
        if ((PhotonNetwork.IsMasterClient && collider.tag.Equals("Master"))
            || (!PhotonNetwork.IsMasterClient && collider.tag.Equals("Client")))
        {
            TurncountTrig.SetActive(true);
            HalfLapTrig.SetActive(false);

            if (PhotonNetwork.IsMasterClient && GameManager.playerTurncounts[0] == 2)
                LapCompleteTrig.SetActive(true);
            else if (!PhotonNetwork.IsMasterClient && GameManager.playerTurncounts[1] == 2)
                LapCompleteTrig.SetActive(true);
        }
    }
}
