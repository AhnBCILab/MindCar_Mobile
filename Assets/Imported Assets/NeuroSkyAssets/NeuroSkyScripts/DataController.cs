using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

//Added for file IO
using System.IO;
using System.Threading;

//public class DisplayData : MonoBehaviour {
public class DataController : MonoBehaviour
{
    //public GameObject Car;

    public Texture2D[] signalIcons;

    private float indexSignalIcons = 1;
    private bool enableAnimation = false;
    private float animationInterval = 0.06f;

    ThinkGearController controller;
    TimeStampManage TM;

    //private int Raw = 0;
    public int Raw = 0;
    //private double Raw = 0;
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
    //private float HighGamma = 0.0f;
    public float HighGamma = 0.0f;

    //private int Algo_Attention = 0;
    public int Algo_Attention = 0;
    private int Algo_Meditation = 0;
    private float Algo_Delta = 0.0f;
    //public float Algo_Delta = 0.0f;
    private float Algo_Theta = 0.0f;
    private float Algo_Alpha = 0.0f;
    //private float Algo_Beta = 0.0f;
    public float Algo_Beta = 0.0f;
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
    public List<double> RawList = new List<double>();

    //false: (1) Mindwave Beta Mode
    //true: (2) MindCar Beta Mode
    bool EEGMode = false;
    //bool EEGMode = true;
    string path = "default";
    double BetaPower = 0;
    int measure_cnt = 0;

    /*
    void Awake()
    {
        controller = GameObject.Find("ThinkGear").GetComponent<ThinkGearController>();
    }
    */

    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("ThinkGear").GetComponent<ThinkGearController>();
        TM = GameObject.Find("TimeStampManage").GetComponent<TimeStampManage>();

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


        UnityThinkGear.Init(true);

        #if UNITY_IPHONE
			clearDataArr();
			UnityThinkGear.ScanDevice();
			showListViewFlag = true;

        #elif UNITY_ANDROID
            UnityThinkGear.StartStream();

        #endif

        //set parameters
        //**WARNING** WINDOW SIZE CAN ONLY BE POWER OF 2
        int Window = 64;
        int Overlap = 50; //Percentage of Overlap
        path = Application.persistentDataPath;

        /*
        if (EEGMode == true)
        {
            var t = new Thread(() => BetaPreproc(Window, Overlap));
            t.Start();
        }
        */

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
    {
        if (measure_cnt == 0)
        {
            TM.SaveTime();
            TM.SaveEvent(2);
            measure_cnt++;
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
    /*
    public float getAlgo_Beta()
    {
        return Algo_Beta;
    }
    */

    public double getAlgo_Beta()
    {
        //return Algo_Beta;

        if (EEGMode == false)
            return Algo_Beta;
        else return BetaPower;
    }

    /*
    void BetaPreproc(int Window, int Overlap)
    {
        int cnt = 1;
        List<double> WindowData = new List<double>(new double[Window]);
        double[] BandPassList = new double[Window];

        while (true)
        {
            if (RawList.Count >= Window * cnt)
            {
                //1. Get Raw data which have size of window
                int j = 0;
                for (int i = (Window * (cnt - 1)); i < (Window * cnt); i++)
                {
                    WindowData[j] = RawList[i];
                    j++;
                }
                cnt++;

                //2. Bandpassfilter the data
                BandPassList = BandPassFilter(WindowData, BandPassList);

                //3. Calculate BetaPower and add to List
                BetaPower = calculatePower(BandPassList);
                //PowerArr.Add(BetaPower);

            }
        }

    }
    */

        /*
    double[] BandPassFilter(List<double> RawList, double[] BandPassList)
    {
        //Tell frequency band
        int fsl = 12;
        int fsh = 18;
        int fs = 512;
        int fl = (RawList.Count * fsl / fs) - 1;
        int fh = (RawList.Count * fsh / fs) + 1;

        if (fl <= 0) fl = 1;

        //Real data with raw data
        double[] Real = RawList.ToArray();
        //Imagine data with 0
        //double[] Imagine = Enumerable.Repeat<double>(0, raw_arr.Length).ToArray<double>();
        double[] Imagine = new double[Real.Length]; //Real.Length should be equal to window size

        // 1. FFT
        FFT ArrFFT = new FFT(64);
        ArrFFT.fft(Real, Imagine);

        // 2. Set data which are not in frequency domain to 0
        for (int i = 0; i < fl - 1; i++)
        {
            Real[i] = 0;
            Imagine[i] = 0;
        }

        for (int i = fh; i < Real.Length; i++)
        {
            Real[i] = 0;
            Imagine[i] = 0;
        }

        // 3. IFFT 
        //result = FFT.calculateIFFT(real_FFT, imaginary_FFT, real_FFT.Length);
        //double[] real_IFFT = result.getReal();
        //double[] imaginary_IFFT = result.getImaginary();

        //BandPassList = real_IFFT;
        BandPassList = Real;

        return BandPassList;

    }
    */

    double calculatePower(double[] List)
    {
        //double[] numArray = new double[List.Count];
        double[] numArray = new double[List.Length];
        double resultPower = 0;

        //Calculate power for every element and sum them
        for (int index = 0; index < numArray.Length; ++index)
        {
            numArray[index] = Math.Pow(List[index], 2.0);
            resultPower += numArray[index];
        }
        resultPower = resultPower / numArray.Length;
        resultPower = Math.Pow(resultPower, 0.5);
        return resultPower;
    }

    public void ResetRaw()
    {
        RawList.Clear();
    }

    public void SaveRawData(string Mode)
    {
        // 1.TO CREATE FOLDER
        if (!Directory.Exists(path + "/Rawdata"))
        {
            Directory.CreateDirectory(path + "/Rawdata");
        }

        //2. TO SAVE DATA
        FileStream f = new FileStream(path + "/Rawdata/" + "RawList_"+ Mode + System.DateTime.Now.ToString("_MM_dd") + ".txt", FileMode.Create, FileAccess.Write);
        BinaryWriter b = new BinaryWriter(f);
        //StreamWriter b = new StreamWriter(f, System.Text.Encoding.ASCII);

        foreach (double element in RawList)
        {
            //Console.WriteLine(prime);
            b.Write(element);
        }
        //RawList.Clear();

        b.Close();
        f.Close();

        ResetRaw(); // (//RawList.Clear();
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
/*
public class FFT
{

    int n, m;

    // Lookup tables. Only need to recompute when size of FFT changes.
    double[] cos;
    double[] sin;

    public FFT(int n)
    {
        this.n = n;
        this.m = (int)(Math.Log(n) / Math.Log(2));

        // Make sure n is a power of 2
        //if (n != (1 << m))
        //throw new RuntimeException("FFT length must be power of 2");

        // precompute tables
        cos = new double[n / 2];
        sin = new double[n / 2];

        for (int i = 0; i < n / 2; i++)
        {
            cos[i] = Math.Cos(-2 * Math.PI * i / n);
            sin[i] = Math.Sin(-2 * Math.PI * i / n);
        }

    }

    public void fft(double[] x, double[] y)
    {
        int i, j, k, n1, n2, a;
        double c, s, t1, t2;

        // Bit-reverse
        j = 0;
        n2 = n / 2;
        for (i = 1; i < n - 1; i++)
        {
            n1 = n2;
            while (j >= n1)
            {
                j = j - n1;
                n1 = n1 / 2;
            }
            j = j + n1;

            if (i < j)
            {
                t1 = x[i];
                x[i] = x[j];
                x[j] = t1;
                t1 = y[i];
                y[i] = y[j];
                y[j] = t1;
            }
        }

        // FFT
        n1 = 0;
        n2 = 1;

        for (i = 0; i < m; i++)
        {
            n1 = n2;
            n2 = n2 + n2;
            a = 0;

            for (j = 0; j < n1; j++)
            {
                c = cos[a];
                s = sin[a];
                a += 1 << (m - i - 1);

                for (k = j; k < n; k = k + n2)
                {
                    t1 = c * x[k + n1] - s * y[k + n1];
                    t2 = s * x[k + n1] + c * y[k + n1];
                    x[k + n1] = x[k] - t1;
                    y[k + n1] = y[k] - t2;
                    x[k] = x[k] + t1;
                    y[k] = y[k] + t2;
                }
            }
        }
    }
}
*/
