using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.IO.Ports;

public class Movement : Singleton <Movement> {
    public float speed = 5;
    public float offset = 15;
    public static string koncentric_USB_Port = "COM6";
    public static string slider_USB_Port = "COM5";
    public GameObject CF_check;
    public GameObject gain_Check;
    public GameObject q_check;
    private float scaleSize = 3;
    public float multiplier = 0.01f;
    public GameObject target;
    public GameObject user;
    public GameObject greenTarget;
    private int selectedBand = 0;
    public Dropdown dropdown;
    public Toggle[] toggles;
    public static float FreqMIN = 30;
    public static float FreqMAX = 850;
    public static float gainMIN = 30;
    public static float gainMAX = 275;
    public static float qMIN = 1;
    public static float qMAX = 5;


    SerialPort serial1 = new SerialPort(koncentric_USB_Port, 9600);
    SerialPort serial2 = new SerialPort(slider_USB_Port, 9600);
    public float[] center_Frequencies = new float[5];
    public float[] Q_values = new float[5]; 
    public float[] gain = new float[5];
    public float[] target_center_Frequencies = new float[5];
    public float[] target_Q_values = new float[5];
    public float[] target_gain = new float[5];
    string value;

    //----------------------------------------------
    // Send B1, B2 ect with sendString(): to change band
    // Send Gain + number to change gain for that band
    // Send Q + number to change Q-value for that band
    // Send CF + number to change center frequency for that band
    // Send LP or HP + number to change the High-pass/Low-pass filter.
    //----------------------------------------------
    public string IP = "127.0.0.1";
    public int port = 9001;
    private IPEndPoint remoteEndPoint;
    private UdpClient client;

    // Use this for initialization
    private void Start() {
        Reset();
        initPDConnection(IP);
        initTargetLocations();
       serial1.Open();
       serial1.ReadTimeout = 100;
        serial2.Open();
        serial2.ReadTimeout = 100;
        SelectBand(selectedBand);

    }

    // Update is called once per frame
    private void Update() {
        UpdatePosition();
        TurnGreen();
        DisplayCheck();
        UpdateSelectedBand();
        WhichBandsAreCompleted();
    }

    void UpdateSelectedBand()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            print("Selected Band: " + 1);
            selectedBand = 0;
            SelectBand(0);
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            print("Selected Band: " + 1);
            selectedBand = 1;
            SelectBand(1);
        }
        if (Input.GetKey(KeyCode.Alpha3)) {
            print("Selected Band: " + 2);
            selectedBand = 2;
            SelectBand(2);
        }
        if (Input.GetKey(KeyCode.Alpha4)) {
            print("Selected Band: " + 3);
            selectedBand = 3;
            SelectBand(3);
        }
        if (Input.GetKey(KeyCode.Alpha5)) {
            print("Selected Band: " + 4);
            selectedBand = 4;
            SelectBand(4);
        }
    }

    void SelectBand(int band_nr)
    {
        for (int i = 0; i < user.transform.childCount; i++)
        {
            user.transform.GetChild(i).gameObject.SetActive(false);
            target.transform.GetChild(i).gameObject.SetActive(false);
        }
        user.transform.GetChild(band_nr).gameObject.SetActive(true);
        target.transform.GetChild(band_nr).gameObject.SetActive(true);
        initTargetLocations();
        ResetKnobs();
    }
    public void Reset()
    {
        ShuffleArray(target_center_Frequencies);
        ShuffleArray(target_gain);
        ShuffleArray(target_Q_values);
        ResetPosition();
        selectedBand = 0;
        SelectBand(selectedBand);
    }

    private void UpdatePosition() {
        //For Mouse
        if (dropdown.value == 0) {
            MouseMovement();
        }
        //For Knobsception movement
        if (dropdown.value == 1) {
            KnobsceptionMovement();

        }
        //For Sliders movement
        if (dropdown.value == 2) {
            SlidersMovement();
        }
        //For keyboard movement
        if (dropdown.value == 3) {
            KeyboardMovement();
        }
    }

    void DisplayCheck() {
        if (center_Frequencies[selectedBand] < target.transform.GetChild(selectedBand).transform.position.x + offset &&
            center_Frequencies[selectedBand] > target.transform.GetChild(selectedBand).transform.position.x - offset) {
            CF_check.gameObject.SetActive(true);
        }
        else {
            CF_check.gameObject.SetActive(false);
        }
        if (gain[selectedBand] < target.transform.GetChild(selectedBand).transform.position.y + offset &&
            gain[selectedBand] > target.transform.GetChild(selectedBand).transform.position.y - offset) {
            gain_Check.gameObject.SetActive(true);
        } else {
            gain_Check.gameObject.SetActive(false);
        }
        if (Q_values[selectedBand] < target.transform.GetChild(selectedBand).transform.localScale.x + offset/100 &&
            Q_values[selectedBand] > target.transform.GetChild(selectedBand).transform.localScale.x - offset/100) {
            q_check.gameObject.SetActive(true);
        } else {
            q_check.gameObject.SetActive(false);
        }
    }

    bool isTargetHit()
    {
        if (center_Frequencies[selectedBand] < target.transform.GetChild(selectedBand).transform.position.x + offset &&
            center_Frequencies[selectedBand] > target.transform.GetChild(selectedBand).transform.position.x - offset &&
            gain[selectedBand] < target.transform.GetChild(selectedBand).transform.position.y + offset &&
            gain[selectedBand] > target.transform.GetChild(selectedBand).transform.position.y - offset &&
            Q_values[selectedBand] < target.transform.GetChild(selectedBand).transform.localScale.x + offset/100 &&
            Q_values[selectedBand] > target.transform.GetChild(selectedBand).transform.localScale.x - offset/100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    float getRandomYPosition()
    {
        float newNumber;
        newNumber = UnityEngine.Random.Range(gainMIN, gainMAX);
        return newNumber;
    }
    float getRandomSize()
    {
        float newNumber;
        newNumber = UnityEngine.Random.Range(qMIN, qMAX);
        return newNumber;
    }
    public static void ShuffleArray<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }
    void TurnGreen()
    {
        float green_Y = greenTarget.transform.position.y;
        if (isTargetHit() && green_Y < 105)
        {
            green_Y += 5;
        } else if (green_Y > -100)
        {
            green_Y -= 5;
        }
        if (green_Y == 105)
        {
            greenTarget.transform.position = new Vector3(greenTarget.transform.position.x, -100, -120);
            green_Y = -100; 
        }
        greenTarget.transform.position = new Vector3(greenTarget.transform.position.x, green_Y, -120);
    }

    public void WhichBandsAreCompleted() {
        for (int i = 0; i < 5; i++) {
            if (center_Frequencies[i] < target.transform.GetChild(i).transform.position.x + offset &&
                center_Frequencies[i] > target.transform.GetChild(i).transform.position.x - offset &&
                gain[i] < target.transform.GetChild(i).transform.position.y + offset &&
                gain[i] > target.transform.GetChild(i).transform.position.y - offset &&
                Q_values[i] < target.transform.GetChild(i).transform.localScale.x + offset/100 &&
                Q_values[i] > target.transform.GetChild(i).transform.localScale.x - offset/100) {
                toggles[i].isOn = true;
            }
            else {
                toggles[i].isOn = false;
            }
        }
    }

    void initPDConnection(string ipInput) {
        IP = ipInput;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    //Used for sending data
    private void sendString(string message) {

        try {
            byte[] data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        } catch (Exception err) {
            print(err.ToString());
        }
    }

    private void initTargetLocations()
    {
        target.transform.GetChild(selectedBand).transform.position = new Vector3(target_center_Frequencies[selectedBand], target_gain[selectedBand], 0);
        target.transform.GetChild(selectedBand).transform.localScale = new Vector3(target_Q_values[selectedBand], scaleSize, 0);
        print("Placed @ " + target_center_Frequencies[selectedBand] + "," + target_gain[selectedBand]);
        print("Actual placement: " + target.transform.GetChild(selectedBand).transform.position.x + "," +
              target.transform.GetChild(selectedBand).transform.position.y);
    }

    private void KeyboardMovement() {
        if (Input.GetKey(KeyCode.A) && center_Frequencies[selectedBand] > FreqMIN) {
            center_Frequencies[selectedBand] -= speed;
            if (selectedBand == 0)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.349f, 1.769f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 1)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.506f, 1.995f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 2)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.650f, 2.187f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 3)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.842f, 2.456f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 4)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.995f, 2.692f, center_Frequencies[selectedBand]));
            }
        }
        if (Input.GetKey(KeyCode.D) && center_Frequencies[selectedBand] < FreqMAX) {
            center_Frequencies[selectedBand] += speed;
            if (selectedBand == 0)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.349f, 1.769f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 1)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.506f, 1.995f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 2)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.650f, 2.187f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 3)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.842f, 2.456f, center_Frequencies[selectedBand]));
            }
            if (selectedBand == 4)
            {
                sendString("CF " + scale(FreqMIN, FreqMAX, 1.995f, 2.692f, center_Frequencies[selectedBand]));
            }
        }
        if (Input.GetKey(KeyCode.S) && gain[selectedBand] > gainMIN) {
            gain[selectedBand] -= speed;
            sendString("Q " + scale(gainMIN, gainMAX, -30, 30, gain[selectedBand]));
        }
        if (Input.GetKey(KeyCode.W) && gain[selectedBand] < gainMAX) {
            gain[selectedBand] += speed;
            sendString("Q " + scale(gainMIN, gainMAX, -30, 30, gain[selectedBand]));
        }
        if (Input.GetKey(KeyCode.E) && Q_values[selectedBand] < qMAX) {
            Q_values[selectedBand] += multiplier;
            sendString("Gain " + scale(qMIN, qMAX, 0, 100, Q_values[selectedBand]));
        }
        if (Input.GetKey(KeyCode.Q) && Q_values[selectedBand] > qMIN) {
            Q_values[selectedBand] -= multiplier;
            sendString("Gain " + scale(qMIN, qMAX, 0, 100, Q_values[selectedBand]));
        }
        user.transform.GetChild(selectedBand).transform.position = new Vector3(center_Frequencies[selectedBand], gain[selectedBand], 0);
        user.transform.GetChild(selectedBand).transform.localScale = new Vector3(Q_values[selectedBand], scaleSize, 0);
        this.transform.position = new Vector3(center_Frequencies[selectedBand], this.transform.position.y, 0);

    }

    void MouseMovement() {
        center_Frequencies[selectedBand] = CFKnob.CF_value;
        gain[selectedBand] = GainKnob.gain_value;
        Q_values[selectedBand] = QKnob.Q_Value;
        sendString("Q " + scale(gainMIN, gainMAX, -30, 30, gain[selectedBand]));
        sendString("Gain " + scale(qMIN, qMAX, 0, 100, Q_values[selectedBand]));
        if (selectedBand == 0)
        {
            sendString("CF " + scale(FreqMIN, FreqMAX, 1.349f, 1.769f, center_Frequencies[selectedBand]));
        }
        if (selectedBand == 1)
        {
            sendString("CF " + scale(FreqMIN, FreqMAX, 1.506f, 1.995f, center_Frequencies[selectedBand]));
        }
        if (selectedBand == 2)
        {
            sendString("CF " + scale(FreqMIN, FreqMAX, 1.650f, 2.187f, center_Frequencies[selectedBand]));
        }
        if (selectedBand == 3)
        {
            sendString("CF " + scale(FreqMIN, FreqMAX, 1.842f, 2.456f, center_Frequencies[selectedBand]));
        }
        if (selectedBand == 4)
        {
            sendString("CF " + scale(FreqMIN, FreqMAX, 1.995f, 2.692f, center_Frequencies[selectedBand]));
        }
        user.transform.GetChild(selectedBand).transform.position = new Vector3(center_Frequencies[selectedBand], gain[selectedBand], 0);
        user.transform.GetChild(selectedBand).transform.localScale = new Vector3(Q_values[selectedBand], scaleSize, 0);
        this.transform.position = new Vector3(center_Frequencies[selectedBand], this.transform.position.y, 0);
    }

    private void KnobsceptionMovement() {
           if (true)
        {
            try
            {
                value = serial1.ReadLine();

                if (value == "ENCO1.1" && center_Frequencies[selectedBand] > FreqMIN) {
                    //  print("Encoder 1 negative");
                    center_Frequencies[selectedBand] -= speed*4.5f;
                    if (selectedBand == 0)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.349f, 1.769f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 1)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.506f, 1.995f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 2)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.650f, 2.187f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 3)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.842f, 2.456f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 4)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.995f, 2.692f, center_Frequencies[selectedBand]));
                    }
                }
                if (value == "ENCO1.2" && center_Frequencies[selectedBand] < FreqMAX) {
                    // print("Encoder 1 positive");
                    center_Frequencies[selectedBand] += speed*4.5f;
                    if (selectedBand == 0)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.349f, 1.769f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 1)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.506f, 1.995f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 2)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.650f, 2.187f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 3)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.842f, 2.456f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 4)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.995f, 2.692f, center_Frequencies[selectedBand]));
                    }
                }

                if (value == "ENCO2.1" && gain[selectedBand] > gainMIN) {
                    //   print("Encoder 2 negative");
                    gain[selectedBand] -= speed*2;
                    sendString("Q " + scale(gainMIN, gainMAX, -30, 30, gain[selectedBand]));
                }
                if (value == "ENCO2.2" && gain[selectedBand] < gainMAX) {
                    //   print("Encoder 2 positive");
                    gain[selectedBand] += speed*2;
                    sendString("Q " + scale(gainMIN, gainMAX, -30, 30, gain[selectedBand]));
                }

                if (value == "ENCO3.1" && Q_values[selectedBand] > qMIN)
                {
                    //   print("Encoder 2 negative");
                    Q_values[selectedBand] -= multiplier;
                    sendString("Gain " + scale(qMIN, qMAX, 0, 100, Q_values[selectedBand]));
                }
                if (value == "ENCO3.2" && Q_values[selectedBand] < qMAX)
                {
                    //   print("Encoder 2 positive");
                    Q_values[selectedBand] += multiplier;
                    sendString("Gain " + scale(qMIN, qMAX, 0, 100, Q_values[selectedBand]));
                }

                if (value == "B1") {
                    //print("button 1 pressed");
                    sendString("B1");
                    selectedBand = 0;
                    SelectBand(0);
                }
                if (value == "B2")
                {
                    //print("button 1 pressed");
                    sendString("B2");
                    selectedBand = 1;
                    SelectBand(1);
                }
                if (value == "B3")
                {
                    //print("button 1 pressed");
                    sendString("B3");
                    selectedBand = 2;
                    SelectBand(2);
                }
                if (value == "B4")
                {
                    //print("button 1 pressed");
                    sendString("B4");
                    selectedBand = 3;
                    SelectBand(3);
                }
                if (value == "B5")
                {
                    //print("button 1 pressed");
                    sendString("B5");
                    selectedBand = 4;
                    SelectBand(4);
                }
            }
            catch (System.Exception){
            }}


        user.transform.GetChild(selectedBand).transform.position = new Vector3(center_Frequencies[selectedBand], gain[selectedBand], 0);
        user.transform.GetChild(selectedBand).transform.localScale = new Vector3(Q_values[selectedBand], scaleSize, 0);
        this.transform.position = new Vector3(center_Frequencies[selectedBand], this.transform.position.y, 0);

    }

    private void SlidersMovement() {
        if (true)
        {
            try
            {
                value = serial2.ReadLine();
                if (value == "ENCO1.1" && gain[selectedBand] > gainMIN)
                {
                    //   print("Encoder 2 negative");
                    gain[selectedBand] -= speed * 2;
                    sendString("Q " + scale(gainMIN, gainMAX, -30, 30, gain[selectedBand]));
                }
                if (value == "ENCO1.2" && gain[selectedBand] < gainMAX)
                {
                    //   print("Encoder 2 positive");
                    gain[selectedBand] += speed * 2;
                    sendString("Q " + scale(gainMIN, gainMAX, -30, 30, gain[selectedBand]));
                }
                if (value == "ENCO2.1" && Q_values[selectedBand] > qMIN)
                {
                    //   print("Encoder 2 negative");
                    Q_values[selectedBand] -= multiplier;
                    sendString("Gain " + scale(qMIN, qMAX, 0, 100, Q_values[selectedBand]));
                }
                if (value == "ENCO2.2" && Q_values[selectedBand] < qMAX)
                {
                    //   print("Encoder 2 positive");
                    Q_values[selectedBand] += multiplier;
                    sendString("Gain " + scale(qMIN, qMAX, 0, 100, Q_values[selectedBand]));
                }

                if (value != "ENCO2.2" && value != "ENCO2.1" && value != "ENCO1.2" && value != "ENCO1.1" && value != "B1" && value != "B2" && value != "B3" && value != "B4" && value != "B5")
                   {
                       //print("SLIDER" + value);
                    
                    center_Frequencies[selectedBand] = scale(550, 10, FreqMIN, FreqMAX, float.Parse(value));
                    if (selectedBand == 0)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.349f, 1.769f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 1)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.506f, 1.995f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 2)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.650f, 2.187f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 3)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.842f, 2.456f, center_Frequencies[selectedBand]));
                    }
                    if (selectedBand == 4)
                    {
                        sendString("CF " + scale(FreqMIN, FreqMAX, 1.995f, 2.692f, center_Frequencies[selectedBand]));
                    }
                }
                if (value == "B1")
                {
                    //print("button 1 pressed");
                    sendString("B1");
                    selectedBand = 0;
                    SelectBand(0);
                }
                if (value == "B2")
                {
                    //print("button 1 pressed");
                    sendString("B2");
                    selectedBand = 1;
                    SelectBand(1);
                }
                if (value == "B3")
                {
                    //print("button 1 pressed");
                    sendString("B3");
                    selectedBand = 2;
                    SelectBand(2);
                }
                if (value == "B4")
                {
                    //print("button 1 pressed");
                    sendString("B4");
                    selectedBand = 3;
                    SelectBand(3);
                }
                if (value == "B5")
                {
                    //print("button 1 pressed");
                    sendString("B5");
                    selectedBand = 4;
                    SelectBand(4);
                }
            }
            catch (System.Exception)
            {
            }
        }


        user.transform.GetChild(selectedBand).transform.position = new Vector3(center_Frequencies[selectedBand], gain[selectedBand], 0);
        user.transform.GetChild(selectedBand).transform.localScale = new Vector3(Q_values[selectedBand], scaleSize, 0);
        this.transform.position = new Vector3(center_Frequencies[selectedBand], this.transform.position.y, 0);
    }

    public void ResetPosition() {
        for (int i = 0; i < 5; i++) {
            center_Frequencies[i] = 90.0f;
            gain[i] = 30.0f;
            Q_values[i] = 3.0f;
        }
        ResetKnobs();
    }

    public static void ResetKnobs() {
        CFKnob.CF_value = 90.0f;
        QKnob.Q_Value = 3.0f;
        GainKnob.gain_value = 30.0f;
        QKnob.Instance.ResetWheel();
        GainKnob.Instance.ResetWheel();
        CFKnob.Instance.ResetWheel();
    }
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}
