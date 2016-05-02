using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class CanvasScript : Singleton<CanvasScript> {
    //Buttons
    public Button start;
    public Button ready;
    public Button finish_button;
    //Texts
    public Text timerText;
    public Text scoreText;
    public Text test_text;
    public Text finish_text;
    //All other things
    //public Canvas knobCanvas;
    public InputField inputText;
    public Dropdown dropdown;
    public RawImage background;
    private IPEndPoint remoteEndPoint;
    private UdpClient client;
    public Toggle doTrailTest;
    //Varibles
    public float countdownMili = 300.0f;
    private float playingTime = 0f;
    private float timeLeft;
    private float finishtime;
    public int port = 9001;
    public int actual_test = 5;
    public int initial_test = 2;
    private int countValue;
    public int state;
    public string IP = "127.0.0.1";


    // Use this for initialization
    void Start() {
        PrepState0();
        reset();
        initPDConnection(IP);
        start.onClick.AddListener(() => {
            if (doTrailTest.isOn) {
                PrepState1();
            } else {
                PrepState1();
                PrepState5();
            }
        });
        finish_button.onClick.AddListener(() => {
            PrepState0();
        });
        ready.onClick.AddListener(() => {
            PrepState4();
        });
    }

    // Update is called once per frame
    void Update() {
        PlayState(state);
    }

    //Handling the different states
    public void PlayState(int switch_value) {
        switch (switch_value) {
            //Finish Screen
            case 6:
                break;
            //Doing the real test
            case 5:
                playingTime += Time.deltaTime;
                countValue = 0;
                for (int i = 0; i < 5; i++) {
                    if (CheckListScript.Instance.isBandsActive[i]) {
                        countValue++;
                        print("Count: " + countValue);
                    }
                }
                if (countValue > 4) {
                    PrepState6();
                }
                break;
            //Countdown screen for the real test
            case 4:
                timeLeft -= Time.deltaTime;
                timerText.text = "Starting the real test in: " + Mathf.Round(timeLeft);
                if (timeLeft < 0) {
                    PrepState5();
                }
                break;
            case 3:
                break;
            //Trail Test
            case 2:
                playingTime += Time.deltaTime;
                countValue = 0;
                for (int i = 0; i < 5; i++) {
                    if (CheckListScript.Instance.isBandsActive[i]) {
                        countValue++;
                    }
                }
                if (countValue > 1) {
                    PrepState3();
                }
                break;
            //Countdown for trail test
            case 1:
                timeLeft -= Time.deltaTime;
                timerText.text = "Starting trail test in: " + Mathf.Round(timeLeft);
                if (timeLeft <= 0) {
                    PrepState2();
                }
                break;
            //Start screen
            case 0:
                break;
            //If something went wrong
            default:
                print("Can't find state");
                break;
        }
    }

    //Send data to pure data
    private void sendString(string message) {

        try {
            byte[] data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        } catch (Exception err) {
            print(err.ToString());
        }
    }

    //Instantiate the connrect to pure data
    void initPDConnection(string ipInput) {
        IP = ipInput;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    //Resets everything for a new test
    public void reset() {
        //knobCanvas.gameObject.SetActive(false);
        finish_text.gameObject.SetActive(false);
        timeLeft = countdownMili;
    }

    //Saves the time spend completing the test to a .txt file
    void FileSaver(string fileName) {
        if (fileName == "") {
            fileName = "Unnamed";
        }
        if (File.Exists(fileName)) {
            fileName += "(copy)";
            FileSaver(fileName);

        } else {
            print("Printing to file " + fileName + "...");
            fileName += ".txt";
            var sr = File.CreateText(fileName);
            sr.WriteLine(Mathf.Round(playingTime) + " seconds");
            sr.Close();
        }
    }

    //Preparation for state 0
    public void PrepState0() {
        finish_button.gameObject.SetActive(false);
        doTrailTest.gameObject.SetActive(true);
        finish_text.gameObject.SetActive(false);
        dropdown.gameObject.SetActive(true);
        start.gameObject.SetActive(true);
        inputText.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        state = 0;
    }

    //Preparation for state 1
    public void PrepState1() {
        start.gameObject.SetActive(false);
        dropdown.gameObject.SetActive(false);
        inputText.gameObject.SetActive(false);
        doTrailTest.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        state = 1;
    }

    //Preparation for state 2
    public void PrepState2() {
        sendString("start");
        timerText.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        print("Changing state: " + state);
        timerText.text = " ";
        timeLeft = countdownMili;
        DisplayKnobs();
        state = 2;
    }

    //Preparation for state 3
    public void PrepState3() {
        timerText.gameObject.SetActive(true);
        timerText.text = "Ready for the real test?";
        scoreText.text = " ";
        background.gameObject.SetActive(true);
        playingTime = 0f;
        sendString("close");
        test_text.gameObject.SetActive(false);
        Movement.Instance.Reset();
        ready.gameObject.SetActive(true);
        state = 3;
    }

    //Preparation for state 3
    public void PrepState4() {
        ready.gameObject.SetActive(false);
        Movement.Instance.Reset();
        state = 4;
    }

    //Preparation for state 4
    public void PrepState5() {
        sendString("start");
        timerText.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        test_text.gameObject.SetActive(false);
        timeLeft = countdownMili;
        timerText.text = " ";
        DisplayKnobs();
        state = 5;
    }

    //Preparation for state 5
    public void PrepState6() {
        scoreText.text = " ";
        FileSaver(inputText.text);
        background.gameObject.SetActive(true);
        playingTime = 0f;
        sendString("close");
        test_text.gameObject.SetActive(false);
        Movement.Instance.Reset();
        finish_text.gameObject.SetActive(true);
        finish_button.gameObject.SetActive(true);
        state = 6;
    }

    //Displays the digital knobs, if mouse input is selected in the start screen
    public void DisplayKnobs() {
        if (dropdown.value == 0) {
            //knobCanvas.gameObject.SetActive(true);
        }
    }
}

