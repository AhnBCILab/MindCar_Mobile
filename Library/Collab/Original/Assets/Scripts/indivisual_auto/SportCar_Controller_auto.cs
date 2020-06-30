﻿using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using System;

//Added for file IO & Thread
using System.IO;
using System.Threading;



public class SportCar_Controller_auto : MonoBehaviour
{
    //DataController DC; //= GameObject.Find("DataController").GetComponent<DataController>();

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
    //int BeginningCommmand = 1;  //1
    //int FinalCommmand = 10;     //10


    //---------------------------------- 
    public GameObject CarControls;

    List<float> SpeedList = new List<float>();
    public float speed;
    double Algo_Beta;
    double Algo_Beta_temp;

    float normal_Speed = 4.0f;
    bool trained = false;
    //To save all the betaPower of the race
    List<double> betaArr = new List<double>();
    double betaAvr = 0;
    double betaStd = 0;

    //To measure diff between time
    DateTime sTime;
    DateTime eTime;
    TimeSpan gapTime;
    float gapSec;
    string path = "default";
    //public bool Finish = false;
    public bool Finish;

    //scaling factor
    float[] s = new float[6];


    void Start()
    {
        Finish = false;
        //DC = GameObject.Find("DataController").GetComponent<DataController>();

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
        //Algo_Beta = DC.getAlgo_Beta();
        //SpeedSet();

    }
    // when start() is removed the car dose not start moving.

    public void StopCar()
    {
        Finish = true;
        speed = 0;
        //TurnOffCar();
    }

    void TurnOffCar()
    {
        CarControls.SetActive(false);
    }

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


    double Meas_Angle(Vector3 P1, Vector3 P2, Vector3 P3)
    {
        double a, b, c;
        double Angle, temp;

        a = Math.Sqrt(Math.Pow(P1.x - P3.x, 2) + Math.Pow(P1.z - P3.z, 2));
        b = Math.Sqrt(Math.Pow(P1.x - P2.x, 2) + Math.Pow(P1.z - P2.z, 2));
        c = Math.Sqrt(Math.Pow(P2.x - P3.x, 2) + Math.Pow(P2.z - P3.z, 2));

        temp = (Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c);

        Angle = Math.Acos(temp);
        Angle = Angle * (180 / Math.PI);

        return Angle;
    }

    float kh_get_angle(Vector3 P1, Vector3 P2, Vector3 P3)
    {
        // P1: current car position, P2: next car position way point, P3: next way point
        // Our goal: calculate angle between Line(P1-P2) and Line(P2-P3)
        // we only use x and z location since y(height) is fixed
        float angle;
        float tmp_sinsoidal;
        float numerator, denominator;
        float diff_x_12, diff_x_13;
        float diff_z_12, diff_z_13;
        float kh_epsilon = 0.00001f;
        diff_x_12 = -(P1.x - P2.x);
        diff_x_13 = -(P1.x - P3.x);

        diff_z_12 = -(P1.z - P2.z);
        diff_z_13 = -(P1.z - P3.z);
        // now unit vectors are followings:
        // u_line12 = (diff_x_12, diff_z_12)
        // u_line13 = (diff_x_13, diff_z_13)

        // angle between two lines
        // cos(theta) = {u_line12 * u_line13 (inner product)} / {abs(u_line12) x abs(u_line13)}
        numerator = diff_x_12 * diff_x_13 + diff_z_12 * diff_z_13;
        denominator = Mathf.Sqrt(Mathf.Pow(diff_x_12, 2) + Mathf.Pow(diff_z_12, 2)) * Mathf.Sqrt(Mathf.Pow(diff_x_13, 2) + Mathf.Pow(diff_z_13, 2));
        if (denominator < kh_epsilon)
            denominator = kh_epsilon;
        tmp_sinsoidal = (float)(numerator / denominator);

        angle = Mathf.Acos(tmp_sinsoidal);
        angle = angle * (180 / Mathf.PI); // radian to degress
        return angle;
    }

    

    void Update()
    {
        //Debug.Log("Finish:" + Finish);

        if (Finish == true)
        {
            //speed = 5;
        }

        else
        {
            speed = 20;
        }
            currPosition = transform.position;
            currRotation = transform.rotation;

            CarDefault.volume = 0.2f + (speed / 6f);


            SpeedobarConverter_auto.ShowSpeed(speed, 0, 10);
            // kh end


            if (waypointIndex < waypoints.Length)
            {
                // moving forward
                //transform.position = Vector3.MoveTowards(currPosition, waypoints[waypointIndex], speed);
                // calculating rotation angle

                Vector3 standard = new Vector3(currPosition.x, currPosition.y, currPosition.z + 20);


                Quaternion Right = Quaternion.identity;
                //Right.eulerAngles = new Vector3(0, (float)angle, 0);
                Quaternion Current = Quaternion.identity;

                angle2 = transform.eulerAngles.y;
                Current.eulerAngles = new Vector3(0, angle2, 0);

                float kh_angle = kh_get_angle(currPosition, standard, waypoints[waypointIndex]);
                kh_angle = Mathf.Round(kh_angle * Mathf.Pow(10, 4)) * 0.0001f;

                if (waypoints[waypointIndex].x - currPosition.x < 0)
                {
                    kh_angle = 360.0f - kh_angle; // reverse?
                }
                angle2 = Mathf.Round(angle2 * Mathf.Pow(10, 4)) * 0.0001f;

                Right.eulerAngles = new Vector3(0, kh_angle, 0);


                //transform.rotation = Quaternion.Slerp(Current, Right, Time.deltaTime * 6.0f);
                transform.Translate(new Vector3(0, 0, speed)); // move forward (only z-axis)

                var dir_right = Quaternion.Euler(0, 55.0f, 0) * transform.forward;
                var dir_left = Quaternion.Euler(0, -55.0f, 0) * transform.forward;
                Ray ray = new Ray(this.transform.position, this.transform.forward);

                Ray ray_right = new Ray(this.transform.position, dir_right);
                Ray ray_left = new Ray(this.transform.position, dir_left);
                //transform.ri
                RaycastHit hit_forward; // forward
                RaycastHit hit_right;
                RaycastHit hit_left;
                float diff_distance = 0;

                Physics.Raycast(ray, out hit_forward, 1000);
                Physics.Raycast(ray_right, out hit_right, 1000);
                Physics.Raycast(ray_left, out hit_left, 1000);
                diff_distance = hit_left.distance - hit_right.distance;

                // ------------------ Wheel direction control using Unity Raycast features
                // Raycast shoots layser to the assigned direction and measrues distance
                // Raycast information: forward, forward + right, and forward + left
                // Case:
                // - when left distance is shorter than right distance
                //  : turn right
                // - when right distance is shorter than left distance
                //  : turn left
                // now turning step is optimized in case of speed=3 (normalization is done in terms of 3)

                if (diff_distance > 15.0f) // control sensitivity level
                {
                    transform.Rotate(new Vector3(0, -1.2f * speed / 3.0f, 0)); // turn left with fine steps
                }
                else if (diff_distance < -15.0f) // control sensitivity level
                {
                    transform.Rotate(new Vector3(0, 1.2f * speed / 2.0f, 0)); // turn right with fine steps     
                }



                if (Vector3.Distance(waypoints[waypointIndex], currPosition) <= 0.01f)
                {
                    waypointIndex++;
                    if (waypointIndex == 21)
                        waypointIndex = 0;
                }

                Marker.transform.position = new Vector3(this.transform.position.x, -1290, this.transform.position.z);
            }
        

        /*
        else
        {

            Algo_Beta_temp = DC.getAlgo_Beta();

            //Set the speed only when Algo_Beta of DC was updated
            //if ((Algo_Beta_temp != Algo_Beta) && (Finish == false))
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


            if (waypointIndex < waypoints.Length)
            {
                // moving forward
                //transform.position = Vector3.MoveTowards(currPosition, waypoints[waypointIndex], speed);
                // calculating rotation angle

                Vector3 standard = new Vector3(currPosition.x, currPosition.y, currPosition.z + 20);
                

                Quaternion Right = Quaternion.identity;
                //Right.eulerAngles = new Vector3(0, (float)angle, 0);
                Quaternion Current = Quaternion.identity;
                
                angle2 = transform.eulerAngles.y;
                Current.eulerAngles = new Vector3(0, angle2, 0);

                float kh_angle = kh_get_angle(currPosition, standard, waypoints[waypointIndex]);
                kh_angle = Mathf.Round(kh_angle * Mathf.Pow(10, 4)) * 0.0001f;

                if (waypoints[waypointIndex].x - currPosition.x < 0)
                {
                    kh_angle = 360.0f - kh_angle; // reverse?
                }
                angle2 = Mathf.Round(angle2 * Mathf.Pow(10, 4)) * 0.0001f;

                Right.eulerAngles = new Vector3(0, kh_angle, 0);


                //transform.rotation = Quaternion.Slerp(Current, Right, Time.deltaTime * 6.0f);
                transform.Translate(new Vector3(0, 0, speed)); // move forward (only z-axis)

                var dir_right = Quaternion.Euler(0, 55.0f, 0) * transform.forward;
                var dir_left = Quaternion.Euler(0, -55.0f, 0) * transform.forward;
                Ray ray = new Ray(this.transform.position, this.transform.forward);
                
                Ray ray_right = new Ray(this.transform.position, dir_right);
                Ray ray_left = new Ray(this.transform.position, dir_left);
                //transform.ri
                RaycastHit hit_forward; // forward
                RaycastHit hit_right;
                RaycastHit hit_left;
                float diff_distance = 0;

                Physics.Raycast(ray, out hit_forward, 1000);
                Physics.Raycast(ray_right, out hit_right, 1000);
                Physics.Raycast(ray_left, out hit_left, 1000);
                diff_distance = hit_left.distance - hit_right.distance;

                // ------------------ Wheel direction control using Unity Raycast features
                // Raycast shoots layser to the assigned direction and measrues distance
                // Raycast information: forward, forward + right, and forward + left
                // Case:
                // - when left distance is shorter than right distance
                //  : turn right
                // - when right distance is shorter than left distance
                //  : turn left
                // now turning step is optimized in case of speed=3 (normalization is done in terms of 3)

                if (diff_distance > 15.0f) // control sensitivity level
                {
                    transform.Rotate(new Vector3(0, -1.2f * speed / 3.0f, 0)); // turn left with fine steps
                }
                else if (diff_distance < -15.0f) // control sensitivity level
                {
                    transform.Rotate(new Vector3(0, 1.2f * speed / 2.0f, 0)); // turn right with fine steps     
                }
                


                if (Vector3.Distance(waypoints[waypointIndex], currPosition) <= 0.01f)
                {
                    waypointIndex++;
                    if (waypointIndex == 21)
                        waypointIndex = 0;
                }

                Marker.transform.position = new Vector3(this.transform.position.x, -1290, this.transform.position.z);
            }
        }
        */
        
       
    }


}