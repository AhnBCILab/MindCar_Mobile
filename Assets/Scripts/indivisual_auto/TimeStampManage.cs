using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TimeStampManage : MonoBehaviour
{

    public string path = "default";
    public string folder = "Timedata";
    public string file = "Timestamp";
    public string file2 = "EventType";
    public int count = 10;
    

    void Start()
    {
        //DontDestroyOnLoad(this);
        path = Application.persistentDataPath;

        /*
        DirectoryCreate(path, folder);
        count++;
        SaveTime();
        SaveEvent(count);
        */
        //SaveFirstT();
        //SaveFirstE();
    }

    public static void SaveFirstTime()
    {

    }

    public static void SaveFirstEvent()
    {

    }

    //Check if directory exist, create a new one if it doesn't exist
    public void DirectoryCreate(string path, string folder)
    {
        if (!Directory.Exists(path + "/" + folder))
        {
            Directory.CreateDirectory(path + "/" + folder);
        }                                      
    }
    
    public void SaveFirstT()
    {
        //1. TO SAVE Timestamp
        FileStream f = new FileStream(path + "/" + folder + "/" + file + "_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter b = new BinaryWriter(f);
        b.Write(System.DateTime.Now.ToString("MM-dd-HH:mm:ss.fff "));
        b.Close();
        f.Close();
    }
    

    public void SaveTime()
    {
        //1. TO SAVE Timestamp
        FileStream f = new FileStream(path + "/" + folder + "/" + file + "_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Append, FileAccess.Write);
        BinaryWriter b = new BinaryWriter(f);
        b.Write(System.DateTime.Now.ToString("HH:mm:ss.fff "));
        b.Close();
        f.Close();
    }

    
    public void SaveFirstE()
    {
        //2. TO SAVE EventType
        FileStream f2 = new FileStream(path + "/" + folder + "/" + file2 + "_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter b2 = new BinaryWriter(f2);
        b2.Write(0);
        b2.Close();
        f2.Close();
    }
    

    public void SaveEvent(int type)
    {
        //2. TO SAVE EventType
        FileStream f2 = new FileStream(path + "/" + folder + "/" + file2 + "_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Append, FileAccess.Write);
        BinaryWriter b2 = new BinaryWriter(f2);
        b2.Write(type);
        b2.Close();
        f2.Close();
    }

    //Timestamp when pause game
    void OnApplicationPause()
    {
        SaveTime();
        SaveEvent(7);
    }

}
