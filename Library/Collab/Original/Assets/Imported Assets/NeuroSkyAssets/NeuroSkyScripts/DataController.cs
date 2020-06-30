using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

//Added for file IO
using System.IO;
using System.Threading;
using System.Collections.Generic;


//public class DisplayData : MonoBehaviour {
public class DataController : MonoBehaviour
{
    //public GameObject Car;

    public Texture2D[] signalIcons;

    private float indexSignalIcons = 1;
    private bool enableAnimation = false;
    private float animationInterval = 0.06f;

    ThinkGearController controller;

    //private int Raw = 0;
    private double Raw = 0;
    private int PoorSignal = 200;
    private int Attention = 0;
    private int Meditation = 0;
    private int Blink = 0;
    private float Delta = 0.0f;
    private float Theta = 0.0f;
    private float LowAlpha = 0.0f;
    private float HighAlpha = 0.0f;
    private float LowBeta = 0.0f;
    private float HighBeta = 0.0f;
    private float LowGamma = 0.0f;
    private float HighGamma = 0.0f;

    private int Algo_Attention = 0;
    private int Algo_Meditation = 0;
    private float Algo_Delta = 0.0f;
    private float Algo_Theta = 0.0f;
    private float Algo_Alpha = 0.0f;
    private float Algo_Beta = 0.0f;
    private float Algo_Gamma = 0.0f;

    //newly added
    private Vector3 currPosition;
    private Quaternion currRotation;

    int BeginningCommmand = 1;  //1
    int FinalCommmand = 10;     //10

    //Tommy add 20161020
    private bool showListViewFlag = false;
    private ArrayList deviceList;
    private ArrayList displayedStrArr;
    Vector2 scrollPosition;
    Rect windowRect;

    float rectX = 0;
    float rectY = 0;
    float rectWidth = 0;
    float rectHeight = 0;

    //To save data
    List<double> RawList = new List<double>();
    List<double> BetaList = new List<double>();
    List<double> BetaPowerList = new List<double>();

    double BetaPower = 0;
    int tempcnt = 0;

    //false: (1) Mindwave Beta Mode
    //true: (2) MindCar Beta Mode
    bool EEGMode = false;
    public static bool done = false;
    public static bool saved = false;
    public static string path = "default";

    // Use this for initialization
    void Start()
    {
    #if UNITY_IPHONE
			clearDataArr();
			UnityThinkGear.ScanDevice();
			showListViewFlag = true;

    #elif UNITY_ANDROID
        UnityThinkGear.StartStream();

     #endif

        controller = GameObject.Find("ThinkGear").GetComponent<ThinkGearController>();

        controller.UpdateRawdataEvent += OnUpdateRaw;
        controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
        controller.UpdateAttentionEvent += OnUpdateAttention;
        controller.UpdateMeditationEvent += OnUpdateMeditation;

        controller.UpdateDeltaEvent += OnUpdateDelta;
        controller.UpdateThetaEvent += OnUpdateTheta;

        controller.UpdateHighAlphaEvent += OnUpdateHighAlpha;
        controller.UpdateHighBetaEvent += OnUpdateHighBeta;
        controller.UpdateHighGammaEvent += OnUpdateHighGamma;

        controller.UpdateLowAlphaEvent += OnUpdateLowAlpha;
        controller.UpdateLowBetaEvent += OnUpdateLowBeta;
        controller.UpdateLowGammaEvent += OnUpdateLowGamma;

        controller.UpdateBlinkEvent += OnUpdateBlink;

        controller.UpdateDeviceInfoEvent += OnUpdateDeviceInfo;
        controller.Algo_UpdateAttentionEvent += OnAlgo_UpdateAttentionEvent;
        controller.Algo_UpdateMeditationEvent += OnAlgo_UpdateMeditationEvent;
        controller.Algo_UpdateDeltaEvent += OnAlgo_UpdateDeltaEvent;
        controller.Algo_UpdateThetaEvent += OnAlgo_UpdateThetaEvent;
        controller.Algo_UpdateAlphaEvent += OnAlgo_UpdateAlphaEvent;
        controller.Algo_UpdateBetaEvent += OnAlgo_UpdateBetaEvent; //here is line to update beta value
        controller.Algo_UpdateGammaEvent += OnAlgo_UpdateGammaEvent;



        deviceList = new ArrayList();
        displayedStrArr = new ArrayList();
        rectX = Screen.width / 10;
        rectY = Screen.height / 3;
        rectWidth = Screen.width * 8 / 10;
        rectHeight = Screen.height / 4;

        //set parameters
        int Window = 51;
        int Overlap = 50; //Percentage of Overlap
        path = Application.persistentDataPath;

    }

    void OnAlgo_UpdateAttentionEvent(int value)
    {
        Algo_Attention = value;
    }
    void OnAlgo_UpdateMeditationEvent(int value)
    {
        Algo_Meditation = value;
    }

    void OnAlgo_UpdateDeltaEvent(float value)
    {
        Algo_Delta = value;
    }
    void OnAlgo_UpdateThetaEvent(float value)
    {
        Algo_Theta = value;
    }
    void OnAlgo_UpdateAlphaEvent(float value)
    {
        Algo_Alpha = value;
    }
    void OnAlgo_UpdateBetaEvent(float value)
    {
        Algo_Beta = value;
        if (EEGMode == false)
        {
            BetaPowerList.Add(Algo_Beta);
        }
    }
    void OnAlgo_UpdateGammaEvent(float value)
    {
        Algo_Gamma = value;
    }


    void OnUpdatePoorSignal(int value)
    {
        PoorSignal = value;
        if (value == 0)
        {
            indexSignalIcons = 0;
            enableAnimation = false;
        }
        else if (value == 200)
        {
            indexSignalIcons = 1;
            enableAnimation = false;
        }
        else if (!enableAnimation)
        {
            indexSignalIcons = 2;
            enableAnimation = true;
        }
    }
    void OnUpdateRaw(int value)
    //void OnUpdateRaw(float value)
    {
        if (tempcnt == 0)
        {
            //TimeStampManage.SaveTime();
            //TimeStampManage.SaveEvent(1);
            tempcnt++;
        }
        Raw = value;
        RawList.Add(Raw);
    }
    void OnUpdateAttention(int value)
    {
        Attention = value;
    }
    void OnUpdateMeditation(int value)
    {
        Meditation = value;
    }
    void OnUpdateDelta(float value)
    {
        Delta = value;
    }
    void OnUpdateTheta(float value)
    {
        Theta = value;
    }
    void OnUpdateHighAlpha(float value)
    {
        HighAlpha = value;
    }
    void OnUpdateHighBeta(float value)
    {
        HighBeta = value;
    }
    void OnUpdateHighGamma(float value)
    {
        HighGamma = value;
    }
    void OnUpdateLowAlpha(float value)
    {
        LowAlpha = value;
    }
    void OnUpdateLowBeta(float value)
    {
        LowBeta = value;
    }
    void OnUpdateLowGamma(float value)
    {
        LowGamma = value;
    }

    void OnUpdateBlink(int value)
    {
        Blink = value;
    }

    // New
    public double getAlgo_Beta()
    {
        if (EEGMode == false)
            return Algo_Beta;
        else return BetaPower;
    }

    void OnUpdateDeviceInfo(string deviceInfo)
    {
        //deviceFound deviceInfo = NSF4F1BF;MindWave Mobile;BAFCEB11-2DB6-70B3-B038-B4AD2EFC6309
        // FMGID ; name ; ConnectId
        print("Unity  Test DeviceInfo = " + deviceInfo);
        Add2DeviceListArray(deviceInfo);
    }

    /**
	 *when Fixed Timestep == 0.02 
	 **/
    void FixedUpdate()
    {
        if (enableAnimation)
        {
            if (indexSignalIcons >= 4.8)
            {
                indexSignalIcons = 2;
            }
            indexSignalIcons += animationInterval;
        }

    }

    void DeviceListWindow(int windowID)
    {
        var buttonStyle = new GUIStyle("Button");
        buttonStyle.fontSize = 40;


        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(rectWidth), GUILayout.Height(rectHeight));
        for (int i = 0; i < deviceList.Count; i++)
        {
            if (GUILayout.Button(displayedStrArr[i] + "", buttonStyle))
            {
                print("Click " + deviceList[i]);
        #if UNITY_IPHONE
				UnityThinkGear.ConnectDevice(deviceList[i]+"");
        #endif
                dismissListView();
            }
        }

        // End the scrollview we began above.
        GUILayout.EndScrollView();
    }

    void dismissListView()
    {
        showListViewFlag = false;
        clearDataArr();
    }

    void clearDataArr()
    {
        deviceList.Clear();
        displayedStrArr.Clear();
    }

    //添加到数组，不添加重复的对象
    void Add2DeviceListArray(string element)
    {
        string mfgid = "";
        string name = "";
        string deviceId = "";

        mfgid = element.Split(";"[0])[0];
        name = element.Split(";"[0])[1];
        deviceId = element.Split(";"[0])[2];
        print("Add2DeviceListArray  mfgid : " + mfgid + " name: " + name + " deviceId: " + deviceId);

        int deviceCount = 0;
        deviceCount = deviceList.Count;
        print("deviceCount : " + deviceCount);
        if (deviceCount > 0)
        {
            for (int i = 0; i < deviceList.Count; i++)
            {
                if (deviceList[i] == deviceId)
                {
                    break;
                }
                else
                {
                    displayedStrArr.Add(mfgid + " " + name);
                    deviceList.Add(deviceId);
                    break;
                }

            }

        }
        else
        {
            displayedStrArr.Add(mfgid + " " + name);
            deviceList.Add(deviceId);
        }


        print("deviceList : " + deviceList);
        print("displayedStrArr : " + displayedStrArr);


    }

}
