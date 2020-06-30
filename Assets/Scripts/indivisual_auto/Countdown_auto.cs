using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Countdown_auto : MonoBehaviour {

	public GameObject CountDown;
	public AudioSource GetReady;
	public AudioSource GoAudio;
    public AudioSource StartEngine;
    public AudioSource CarDefault;

	public GameObject LapTimer;
	public GameObject CarControl_Active;
    public GameObject CarControl_Disactive;
    public GameObject Minimap;

	void Start()
	{
        CarControl_Disactive.SetActive(true);
        LapTimer.SetActive(false);
        StartCoroutine(CountStart());
        TurnManager_auto.Turncount = 0;
    }


	IEnumerator CountStart()
	{

		yield return new WaitForSeconds(0.5f);
		CountDown.GetComponent<Text>().text = "3";
		GetReady.Play();
		CountDown.SetActive(true);
		yield return new WaitForSeconds(1);


		CountDown.SetActive(false);
		CountDown.GetComponent<Text>().text = "2";
		GetReady.Play();
		CountDown.SetActive(true);
        yield return new WaitForSeconds(1);
        StartEngine.Play();



		CountDown.SetActive(false);
		CountDown.GetComponent<Text>().text = "1";
		GetReady.Play();
		CountDown.SetActive(true);
		yield return new WaitForSeconds(1);


		CountDown.SetActive(false);
		CountDown.GetComponent<Text>().text = "Go";
        GoAudio.Play();
        CarDefault.Play();
        LapTimer.SetActive(true);
        //Minimap.SetActive(true);

        CarControl_Active.SetActive(true);
	}
}
