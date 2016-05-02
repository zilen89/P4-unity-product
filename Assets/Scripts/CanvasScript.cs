using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Text;



public class CanvasScript : Singleton<CanvasScript>
{
    public float countdownMili = 300.0f;
    public Button start;
    public RawImage background;
    public Text timerText;
    public Text scoreText;
    public InputField inputText;
    private float timeLeft;
    private float finishtime;
    private bool isStarting = false;
    private bool isPlaying = false;
    private string fileName;
    private float playingTime = 0f;
    public Dropdown dropdown;
    public Toggle[] toggles;
    private bool isAllBandsHit = false;
    private bool istestBandsHit = false;
    public int actual_test = 5;
    public int initial_test = 2;
    public string IP = "127.0.0.1";
    public int port = 9001;
    private IPEndPoint remoteEndPoint;
    private UdpClient client;
    public GameObject Q_knob;
    public GameObject CF_knob;
    public GameObject gain_knob;
    public Text test_text;
    public Text finish_text;
    public Text Q_text;
    public Text CF_text;
    public Text gain_text;
    public Boolean istest = true;


    // Use this for initialization
    void Start () {
        reset();
        initPDConnection(IP);
        start.onClick.AddListener(() =>
        {
            isStarting = true;

            start.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(false);
            inputText.gameObject.SetActive(false);
        });
    }
	
	// Update is called once per frame
	void Update () {
	    PlayingHandler();
	    StartScreenHandler();
	    PlayingTimer();
	}
    private void sendString(string message)
    {

        try
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    void initPDConnection(string ipInput)
    {
        IP = ipInput;
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    void PlayingHandler() {
        if (istest)
        {

            for (int i = 0; i < initial_test; i++)
            {
                if (!toggles[i].isOn)
                {
                    break;
                }
                if (i == initial_test - 1)
                {
                    istestBandsHit = true;
                    istest = false;
                }
            }
            if (istestBandsHit && isPlaying)
            {
                scoreText.text = " ";
                background.gameObject.SetActive(true);
                start.gameObject.SetActive(true);
                dropdown.gameObject.SetActive(true);
                inputText.gameObject.SetActive(true);
                playingTime = 0f;
                isPlaying = false;
                isAllBandsHit = false;
                sendString("close");
                //Movement.Reset();
                istestBandsHit = false;
                test_text.gameObject.SetActive(false);
                Movement.Instance.Reset();
            }
        }
        if (!istest)
        {
            if (isPlaying)
            {
                scoreText.text = "Time: " + Mathf.Round(playingTime);
            }

            for (int i = 0; i < actual_test; i++)
            {
                if (!toggles[i].isOn)
                {
                    break;
                }
                if (i == actual_test - 1)
                {
                    isAllBandsHit = true;
                    istest = true;
                }
            }

            if (isAllBandsHit && isPlaying)
            {
                finish_text.gameObject.SetActive(true);
                finishtime -= Time.deltaTime;
                if(finishtime <= 0)
                {
                    Movement.Instance.Reset();
                    scoreText.text = " ";
                    background.gameObject.SetActive(true);
                    start.gameObject.SetActive(true);
                    dropdown.gameObject.SetActive(true);
                    inputText.gameObject.SetActive(true);
                    fileName = inputText.text + ".txt";
                    FileSaver();
                    inputText.text = " ";
                    playingTime = 0f;
                    isPlaying = false;
                    isAllBandsHit = false;
                    sendString("close");
                    isAllBandsHit = false;
                    finishtime = countdownMili;
                    test_text.gameObject.SetActive(true);
                }



                reset();
            }
        }
    }
    public void reset()
    {
        Q_text.gameObject.SetActive(false);
        CF_text.gameObject.SetActive(false);
        gain_text.gameObject.SetActive(false);
        Q_knob.gameObject.SetActive(false);
        CF_knob.gameObject.SetActive(false);
        gain_knob.gameObject.SetActive(false);
        finish_text.gameObject.SetActive(false);
        timeLeft = countdownMili;
    }

    void StartScreenHandler()
    {
        if (isStarting)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = "Starting in: " + Mathf.Round(timeLeft);
            if (timeLeft < 0) {
                //print("Playing...");
                //print("Playing");
                sendString("start");

                background.gameObject.SetActive(false);
                if (dropdown.value == 0) {
                    CF_knob.gameObject.SetActive(true);
                    Q_knob.gameObject.SetActive(true);
                    gain_knob.gameObject.SetActive(true);
                }
                timeLeft = countdownMili;
                timerText.text = " ";
                isStarting = false;
                isPlaying = true;
            }
        }
    }

    void FileSaver()
    {
        if (File.Exists(fileName)) {
            Debug.Log(fileName + " already exists.");
            fileName = inputText.text + "(copy).txt";
        }
        print("Printing to file...");
        var sr = File.CreateText(fileName);
        sr.WriteLine(Mathf.Round(playingTime) + " seconds");
        sr.Close();
    }

    void PlayingTimer()
    {
        if (isPlaying)
        {
            playingTime += Time.deltaTime;
        }
    }
}
