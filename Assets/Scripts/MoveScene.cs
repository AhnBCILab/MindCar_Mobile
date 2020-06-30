using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    TimeStampManage TM;

    void Start()
    {
        TM = GameObject.Find("TimeStampManage").GetComponent<TimeStampManage>();
        //TM.SaveTime();
        //TM.SaveEvent(1);
    }

    void Update()
    {

    }
    public void QuitGame()
    {
        TM.SaveTime();
        TM.SaveEvent(8);
        Application.Quit();
    }

    public void ChangeGameScene_0()
    {
        //TimeStampManage.SaveFirstTime();
        //TimeStampManage.SaveFirstEvent();
        SceneManager.LoadScene("0_Start_View");

    }

    public void ChangeGameScene_1()
    {
        SceneManager.LoadScene("1_Mode");
    }

    public void ChangeGameScene_2_1()
    {
        SceneManager.LoadScene("2_1_Individual_Setting");
    }

    public void ChangeGameScene_2_2()
    {
        SceneManager.LoadScene("2_2_Competition_Setting");
    }

    public void ChangeGameScene_3()
    {
        SceneManager.LoadScene("3_Training");
    }

    public void ChangeGameScene_4_1()
    {
        SceneManager.LoadScene("4_Manual_Individual");
    }

    public void ChangeGameScene_4_2()
    {
        SceneManager.LoadScene("4_Auto_Individual");
    }
    public void ChangeGameScene_4_3()
    {
        SceneManager.LoadScene("competition");
    }

}