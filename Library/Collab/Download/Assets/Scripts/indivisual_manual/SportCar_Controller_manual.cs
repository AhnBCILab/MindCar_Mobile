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

    //---chan's code---//
    List<float> SpeedList = new List<float>();
    float speed;
    double Algo_Beta;
    double Algo_Beta_temp;

    float normal_Speed = 4.0f;
    bool trained = false;
    //To save all the betaPower of the race
    List<double> betaArr = new List<double>();
    double betaAvr = 0;
    double betaStd = 0;
    //int command;

    //To measure diff between time
    DateTime sTime;
    DateTime eTime;
    TimeSpan gapTime;
    float gapSec;
    string path = "default";

    //scaling factor
    float[] s = new float[6];
    //---chan's code---//

    void Awake()
    {
        DC = GameObject.Find("DataController").GetComponent<DataController>();
    }

    void Start()
    {
        waypoints = new Vector3[22];
        //var Client = GameObject.Find("PressController").GetComponent<ForPress>();
        //baseline_from_rest = Client.baseline_from_controller;
        //std_baseline = Client.std_baseline;
        start_time = 0;
        maintain_value = 0;

        //---chan's code---//
        //set path and start time
        sTime = DateTime.Now;
        path = Application.persistentDataPath;

        //set scaling factor
        s[0] = -1.4f;
        s[1] = -0.8f;
        s[2] = -0.2f;
        s[3] = 0.2f;
        s[4] = 0.8f;
        s[5] = 1.4f;

        //Initialize speed
        Algo_Beta = DC.getAlgo_Beta();
        SpeedSet();

        //---chan's code---//
    }
    // when start() is removed the car dose not start moving.

    void SaveTrainingBeta(List<double> valueArray, double betaStd, double betaAvr)
    {

        //check if folder exists
        if (!Directory.Exists(path + "/Betadata"))
        {
            Directory.CreateDirectory(path + "/Betadata");
        }

        FileStream fs = new FileStream(path + "/Betadata/" + "TrainingBetaList_" + System.DateTime.Now.ToString("MM_dd") + ".bin", FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);

        for (int i = 0; i < valueArray.Count; i++)
        {
            bw.Write(valueArray[i]);
        }

        bw.Write(betaStd);
        bw.Write(betaAvr);

        bw.Close();
        fs.Close();
    }

    //---chan's code---//
    void SpeedSet()
    {
        speed = normal_Speed;

        //1. After 30 sec(train done)
        if (trained)
        {
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
            //if ( (Algo_Beta > 0.1) && (Algo_Beta != betaArr[betaArr.Count - 1] )
            if (Algo_Beta > 0.1)
            {
                betaArr.Add(Algo_Beta);
            }

            //Measure time difference
            eTime = DateTime.Now;
            gapTime = eTime - sTime;
            gapSec = gapTime.Seconds;

            //trained-->true when over 30 seconds
            if (gapSec >= 30)
            {
                //DC.SaveTimeStamp(System.DateTime.Now.ToString("mm:ss.fff"), 2);
                //TimeStampManage.SaveTime();
                //TimeStampManage.SaveEvent(2);
                betaAvr = GetAverage(betaArr);
                betaStd = GetStandardDeviation(betaArr, betaAvr);
                trained = true;
                //ToBinary(betaArr);
                //Bin.Start();
                var th = new Thread(() => SaveTrainingBeta(betaArr, betaStd, betaAvr));
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
    //---chan's code---//

    void Update()
    {
        Algo_Beta_temp = DC.getAlgo_Beta();

        //Set the speed only when Algo_Beta of DC was updated
        if (Algo_Beta_temp != Algo_Beta)
        {
            Algo_Beta = Algo_Beta_temp;
            if (Algo_Beta < 0) Algo_Beta = 0;
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