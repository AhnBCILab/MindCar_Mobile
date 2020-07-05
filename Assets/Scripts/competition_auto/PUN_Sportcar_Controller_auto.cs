using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Added for file IO & Thread
using System.IO;
using System.Threading;



public class PUN_Sportcar_Controller_auto : MonoBehaviourPun//, IPunObservable
{
    DataController DC;
    TimeStampManage TM;

    public GameObject camera;
    public GameObject Collider;
    public GameObject Marker;

    private float timePassed;
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
    //---Newly added for MindCar Mobile ver.---//
    public GameObject CarControls;

    //To handle speed of the race
    public List<float> SpeedList = new List<float>();
    public float speed;
    public double Algo_Beta = 0;
    double Algo_Beta_temp = 0;

    float normal_Speed = 5; //4.0f;
    bool trained = false;
    //public bool Finish;

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
    int BeforeCntDown = 0;
    int AfterCntDown = 0;
    int StopCnt = 0;

    //---------------------------------- 

    void Awake()
    {
        //DC = GameObject.Find("DataController").GetComponent<DataController>();
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (photonView.IsMine) Collider.gameObject.tag = "Master";
            else Collider.gameObject.tag = "Client";
        }
        else
        {
            if (photonView.IsMine) Collider.gameObject.tag = "Client";
            else Collider.gameObject.tag = "Master";
        }

        waypoints = new Vector3[22];
        start_time = 0;
        maintain_value = 0;
        if (photonView.IsMine) camera.SetActive(true);

        TM = GameObject.Find("TimeStampManage").GetComponent<TimeStampManage>();
        DC = GameObject.Find("DataController").GetComponent<DataController>();
        DC.ResetRaw();

        //---Newly added for MindCar Mobile ver.---//
        //set path and start time
        sTime = DateTime.Now;
        path = Application.persistentDataPath;
        trained = false;
        //Finish = false;

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
        //---Newly added for MindCar Mobile ver.---//

    }
    // when start() is removed the car dose not start moving.

    public void StopCar()
    {
        //Finish = true;
        //speed = 0;
        SaveSpeedList();
        SaveBetaPowerList();
        DC.SaveRawData("Multi_Auto");
        //*In Multi Mode, Last lap is recoreded in TurncountTrigger script(On contrary, Last lap was not recorded in TurncounTrigger script of Individual Mode)*
        //TM.SaveEvent(6);
        //TM.SaveTime();
        StopCnt++;
        //Directory.CreateDirectory(path + "/TurnOffTest_Multi_" + StopCnt);
        //TurnOffCar();
    }
    /*
    void TurnOffCar()
    {
        //CarControls.SetActive(false);
    }
    */

    void SaveBetaPowerList()
    {
        //check if folder exists
        if (!Directory.Exists(path + "/BetaPower_data"))
        {
            Directory.CreateDirectory(path + "/BetaPower_data");
        }

        FileStream fs = new FileStream(path + "/BetaPower_data/" + "BetaPowerList_Multi_AutoMode_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        //StreamWriter bw = new StreamWriter(fs, System.Text.Encoding.ASCII);

        foreach (double element in betaPowerList)
        {
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

        FileStream fs = new FileStream(path + "/Speed_data/" + "SpeedList_Multi_AutoMode_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        //StreamWriter bw = new StreamWriter(fs,System.Text.Encoding.ASCII);

        foreach (float element in SpeedList)
        {
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

        FileStream fs = new FileStream(path + "/BetaTrainingdata/" + "TrainingBetaList_Multi_AutoMode_" + System.DateTime.Now.ToString("MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);
        //StreamWriter bw = new StreamWriter(fs, System.Text.Encoding.ASCII);

        foreach (double element in valueArray)
        {
            bw.Write(element);
        }

        //bw.Write(betaStd);
        //bw.Write(betaAvr);

        bw.Close();
        fs.Close();
    }

    //---Newly added for MindCar Mobile ver.---//
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
                TM.SaveTime();
                TM.SaveEvent(3);

                betaAvr = GetAverage(betaPowerList);
                betaStd = GetStandardDeviation(betaPowerList, betaAvr);
                trained = true;
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
    //---Newly added for MindCar Mobile ver.---//


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
        //if (!photonView.IsMine) return;

        if (!photonView.IsMine) return;
        //if (!StartManager.StartBool) return;

        if (!StartManager.StartBool)
        {
            // This case is when before having countdown, so stopcar() function is invoked at beginning even though it is not needed..
            if (BeforeCntDown == 0)
            {
                StopCar();
                BeforeCntDown = 1;
            }
            // This case is invoked right after done with 3 laps.
            if (BeforeCntDown == 1 && AfterCntDown == 1)
            {
                StopCar();
                //Just make them into meaningless integer
                BeforeCntDown = -1;
                AfterCntDown = -1;
            }
            return;
        }

        //this line will be called constantly after 3 sec of countdown
        AfterCntDown = 1;

        Algo_Beta_temp = DC.getAlgo_Beta();
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


        if (waypointIndex < waypoints.Length)
        {
            // moving forward
            //transform.position = Vector3.MoveTowards(currPosition, waypoints[waypointIndex], speed);
            // calculating rotation angle

            Vector3 standard = new Vector3(currPosition.x, currPosition.y, currPosition.z + 20);
            /*
            double angle = Meas_Angle( standard, currPosition, waypoints[waypointIndex]);	
            if ( waypoints[waypointIndex].x - currPosition.x < 0)
                angle = 360 - angle;
            */

            Quaternion Right = Quaternion.identity;
            //Right.eulerAngles = new Vector3(0, (float)angle, 0);
            Quaternion Current = Quaternion.identity;
            /*
            if (transform.eulerAngles.y != 0f)
            {
                angle2 = transform.eulerAngles.y;
            }*/
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
            /*
            Ray ray_right = new Ray(this.transform.position, (this.transform.right + this.transform.forward).normalized);
            Ray ray_left = new Ray(this.transform.position, (this.transform.forward - this.transform.right).normalized);
            */
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
            /*
            Debug.Log("forward:"+ hit_forward.distance+" left:" + hit_left.distance + " right:" + hit_right.distance + " diff:" +
                diff_distance+ " speed: "+speed);
            */


            if (Vector3.Distance(waypoints[waypointIndex], currPosition) <= 0.01f)
            {
                waypointIndex++;
                if (waypointIndex == 21)
                    waypointIndex = 0;
            }

            Marker.transform.position = new Vector3(this.transform.position.x, -1290, this.transform.position.z);
        }

    }
}
