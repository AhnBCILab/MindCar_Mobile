using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FirstTimestamp : MonoBehaviour {

    TimeStampManage TM;
    string path = "default";
    string folder = "Timedata";
    string file = "Timestamp";
    string file2 = "EventType";

    // Use this for initialization
    void Start ()
    {
        path = Application.persistentDataPath;
        TM = GameObject.Find("TimeStampManage").GetComponent<TimeStampManage>();
        
        //Record Event only when directory does not exist
        if (!Directory.Exists(path + "/" + folder))
        {
            //Directory.CreateDirectory(path + "/TEST_06_27_1ST");
            Directory.CreateDirectory(path + "/" + folder);
            //TM.DirectoryCreate(path, folder);
            SaveFirstE();
            SaveFirstT();
            //Directory.CreateDirectory(path + "/TEST_06_27_2ND");
        }
        /*
        else
        {
            TM.DirectoryCreate(path, folder);
            TM.SaveFirstE();
            TM.SaveFirstT();
            Directory.CreateDirectory(path + "/TEST_06_26");
        }
        */
    }

    public void SaveFirstT()
    {
        //TO SAVE Timestamp
        FileStream f = new FileStream(path + "/" + folder + "/" + file + "_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter b = new BinaryWriter(f);
        b.Write(System.DateTime.Now.ToString("MM-dd-HH:mm:ss.fff "));
        b.Close();
        f.Close();
    }

    public void SaveFirstE()
    {
        //TO SAVE EventType
        FileStream f2 = new FileStream(path + "/" + folder + "/" + file2 + "_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter b2 = new BinaryWriter(f2);
        b2.Write(0);
        b2.Close();
        f2.Close();
    }

}
