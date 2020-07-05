using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Added for file IO & Thread
using System.IO;
using System.Threading;



public class SportCar_Controller_manual : MonoBehaviour
{
    DataController DC;
    TimeStampManage TM;

    public GameObject Marker;
    private float timePassed;
    // Use this for initialization
    public Vector3[] waypoints;
    private Vector3 currPosition;
    private Quaternion currRotation;
    private int waypointIndex = 0;
    public Transform target;
    private float angle2 = 0;
    public AudioSource CarDefault;

    public List<double> signal = new List<double>();
    public int command;

    public float start_time;
    public float maintain_value;

    //----------------------------------
    //float BeginningCommmand = 1.0f;  //1
    //float FinalCommmand = 10.0f;     //10


    //---------------------------------- 
    //---New for MindCar Mobile ver.---//
    public GameObject CarControls;

    //To handle speed of the race
    public List<float> SpeedList = new List<float>();
    public float speed;
    public double Algo_Beta = 0;
    double Algo_Beta_temp = 0;

    float normal_Speed = 5; //4.0f;
    bool trained = false;
    public bool Finish;

    //To save all the betaPower of the race
    public List<double> betaPowerList = new List<double>();
    double betaAvr = 0;
    double betaStd = 0;

    //To measure diff between time
    DateTime sTime;
    DateTime eTime;
    TimeSpan gapTime;
    float gapSec;
    string path = "default";

    //scaling factor
    float[] s = new float[6];
    //---New for MindCar Mobile ver.---//

    void Awake()
    {
        //DC = GameObject.Find("DataController").GetComponent<DataController>();
    }

    void Start()
    {
        TM = GameObject.Find("TimeStampManage").GetComponent<TimeStampManage>();
        DC = GameObject.Find("DataController").GetComponent<DataController>();
        DC.ResetRaw();

        waypoints = new Vector3[22];
        start_time = 0;
        maintain_value = 0;

        //---New for MindCar Mobile ver.---//
        //set path and start time
        sTime = DateTime.Now;
        path = Application.persistentDataPath;
        trained = false;
        Finish = false;

        TM.SaveTime();
        TM.SaveEvent(1);

        //set scaling factor
        s[0] = -1.4f;
        s[1] = -0.8f;
        s[2] = -0.2f;
        s[3] = 0.2f;
        s[4] = 0.8f;
        s[5] = 1.4f;

        //Initialize speed
        Algo_Beta = DC.getAlgo_Beta();
        Algo_Beta_temp = Algo_Beta;
        SpeedSet();
        //---New for MindCar Mobile ver.---//
    }
    // when start() is removed the car dose not start moving.

    public void StopCar()
    {
        Finish = true;
        speed = 0;
        SaveSpeedList();
        SaveBetaPowerList();
        DC.SaveRawData("Manual");
        TM.SaveEvent(6);
        TM.SaveTime();
        TurnOffCar();
    }

    void TurnOffCar()
    {
        CarControls.SetActive(false);
    }

    void SaveBetaPowerList()
    {
        //check if folder exists
        if (!Directory.Exists(path + "/BetaPower_data"))
        {
            Directory.CreateDirectory(path + "/BetaPower_data");
        }

        FileStream fs = new FileStream(path + "/BetaPower_data/" + "BetaPowerList_Manual_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        //StreamWriter bw = new StreamWriter(fs, System.Text.Encoding.ASCII);

        foreach (double element in betaPowerList)
        {
            //Console.WriteLine(prime);
            bw.Write(element);
        }

        bw.Close();
        fs.Close();
        //TurnOffCar();
    }

    void SaveSpeedList()
    {
        //check if folder exists
        if (!Directory.Exists(path + "/Speed_data"))
        {
            Directory.CreateDirectory(path + "/Speed_data");
        }

        FileStream fs = new FileStream(path + "/Speed_data/" + "SpeedList_Manual_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        //StreamWriter bw = new StreamWriter(fs, System.Text.Encoding.ASCII);

        foreach (float element in SpeedList)
        {
            //Console.WriteLine(prime);
            bw.Write(element);
        }
        
        bw.Close();
        fs.Close();
    }

    void SaveTrainingBeta(List<double> valueArray, double betaStd, double betaAvr)
    {

        //check if folder exists
        if (!Directory.Exists(path + "/BetaTrainingdata"))
        {
            Directory.CreateDirectory(path + "/BetaTrainingdata");
        }

        FileStream fs = new FileStream(path + "/BetaTrainingdata/" + "TrainingBetaList_Manual_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        //StreamWriter bw = new StreamWriter(fs, System.Text.Encoding.ASCII);

        foreach (double element in valueArray)
        {
            //Console.WriteLine(prime);
            bw.Write(element);
        }

        //bw.WriteLine(betaStd);
        //bw.WriteLine(betaAvr);

        bw.Close();
        fs.Close();
    }

    //---New for MindCar Mobile ver.---//
    void SpeedSet()
    {
        speed = normal_Speed;

        //1. After 30 sec(train done)
        if (trained)
        {
            //betaPowerList.Add(Algo_Beta);
            if (Algo_Beta >= 0 && Algo_Beta < betaAvr + betaStd * s[0])
            {
                command = 0;
            }

            else if (Algo_Beta >= betaAvr + betaStd * s[5])
            {
                command = 6;
            }

            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (Algo_Beta >= betaAvr + betaStd * s[i - 1] && Algo_Beta < betaAvr + betaStd * s[i])  // If it was 1, then it's range was (M+std*s[0] ~ M+std*s[1]).
                    {
                        command = i;
                        break;
                    }
                }
            }

            switch (command)
            {

                case 0: speed = 2.7f; break;
                case 1: speed = 3.3f; break;
                case 2: speed = 3.7f; break;
                case 3: speed = 4.0f; break;
                case 4: speed = 4.3f; break;
                case 5: speed = 4.7f; break;
                case 6: speed = 5.3f; break;
                default: break;
            }
        }

        //2. Before 30 sec(train not done)
        else
        {
            //Measure time difference
            eTime = DateTime.Now;
            gapTime = eTime - sTime;
            gapSec = gapTime.Seconds;

            //trained-->true when over 30 seconds
            if (gapSec >= 30)
            {
                // This Event will not be recorded if EEG data were not measure properly
                TM.SaveTime();
                TM.SaveEvent(3);

                betaAvr = GetAverage(betaPowerList);
                betaStd = GetStandardDeviation(betaPowerList, betaAvr);
                trained = true;
                // Thread to save accumulated Beta data during training Mode
                var th = new Thread(() => SaveTrainingBeta(betaPowerList, betaStd, betaAvr));
                th.Start();
            }
        }

        //Add processed speed
        SpeedList.Add(speed);

    }

    //To calculate average
    double GetAverage(List<double> valueArray)
    {
        int valueCount = valueArray.Count;

        if (valueCount == 0)
        {
            return 0d;
        }

        double Average = 0d;

        for (int i = 0; i < valueCount; i++)
        {
            Average += valueArray[i];
        }
        Average /= valueCount;

        return Average;
    }

    //To calculate std
    double GetStandardDeviation(List<double> valueArray, double average)
    {
        int valueCount = valueArray.Count;

        if (valueCount == 0)
        {
            return 0d;
        }

        double standardDeviation = 0d;
        double variance = 0d;

        try
        {
            for (int i = 0; i < valueCount; i++)
            {
                variance += Math.Pow(valueArray[i] - average, 2);
            }
            standardDeviation = Math.Sqrt(SafeDivide(variance, valueCount));
        }

        catch (Exception)
        {
            throw;
        }
        return standardDeviation;
    }

    //used when calculating std
    double SafeDivide(double value1, double value2)
    {
        double result = 0d;

        try
        {
            if ((value1 == 0) || (value2 == 0))
            {
                return 0d;
            }
            result = value1 / value2;
        }

        catch
        {

        }
        return result;
    }
    //---New for MindCar Mobile ver.---//

    void Update()
    {

        Algo_Beta_temp = DC.getAlgo_Beta();

        if (Finish == true)
        {
            //Do nothing when finished race
        }

        else
        {
            //Algo_Beta_temp = DC.getAlgo_Beta();

            //Set the speed only when Algo_Beta of DC was updated
            if (Algo_Beta_temp != Algo_Beta)
            { 
                Algo_Beta = Algo_Beta_temp;
                if (Algo_Beta < 0) Algo_Beta = 0;
                betaPowerList.Add(Algo_Beta);
                SpeedSet();
            }

            currPosition = transform.position;
            currRotation = transform.rotation;

            CarDefault.volume = 0.2f + (speed / 6f);

            SpeedobarConverter_auto.ShowSpeed(speed, 0, 10);
            // kh end

            transform.Translate(new Vector3(0, 0, speed));
            transform.Rotate(new Vector3(0, speed * Input.acceleration.x, 0));

            Marker.transform.position = new Vector3(this.transform.position.x, -1290, this.transform.position.z);
        }
    }


}