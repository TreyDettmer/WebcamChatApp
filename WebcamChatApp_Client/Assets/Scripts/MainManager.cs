using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Managages the chat screen UI
/// </summary>
public class MainManager : MonoBehaviour
{
    public static MainManager instance;
    public InputField messageInputField;
    public GameObject chatPanel, messageObject;
    public GameObject lobbyPanel, lobbyTextObject;
    public TextMeshProUGUI serverAnnouncementText;
    public int maxMessages = 40;
    
    public List<GameObject> messageGameobjects = new List<GameObject>();

    //public delegate void PCMReaderCallback(float[] data);
    //PCMReaderCallback PCMmicDataCallback = MicDataCallback;
    public WebCamTexture webCamTexture;
    [HideInInspector]public ChatterWebcamHandle chatterWebcamHandle;
    
    private Texture2D webCamTexture2D;
    private AudioClip mic;
    public AudioSource audioSource;
    public int micFrequency = 44100;
    public int modifiedFrequency = 22050;
    public float latency = .3f;
    public float webcamFramerate = 20f;
    
    public List<byte[]> webcamLatencyBuffer = new List<byte[]>();
    private string microphoneDevice;

    public bool bHasWebcam = false;
    public bool bHasMic = false;
    bool bWebcamEnabled = false;
    bool bMicMuted= true;

    private int lastSample;
    private float micLatencyTimer = 0f;
    private float webcamLatencyTimer = 0f;
    private float webcamFramerateTimer = 0f;

    public bool bHoldingSpacebar = false;
    private bool bSpacebarRoutineRunning = false;


    /// <summary>
    /// Initialize singleton
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        GuiManager.instance.EnableWebcamOptionsWindow(WebCamTexture.devices);
        GuiManager.instance.EnableMicOptionsWindow(Microphone.devices);


        

    }

    


    /// <summary>
    /// Change the webcam device
    /// </summary>
    /// <param name="_device">webcam device</param>
    public bool ChangeWebcamDevice(string _deviceName)
    {

        if (_deviceName != "")
        {
                
            webCamTexture = new WebCamTexture();
            webCamTexture.requestedHeight = 75;
            webCamTexture.requestedWidth = 100;
            webCamTexture.requestedFPS = webcamFramerate;
            webCamTexture.deviceName = _deviceName;
            bHasWebcam = true;
        }
        else
        {
            bHasWebcam = false;
        }
        
        return true;

    }

    public bool ChangeMicDevice(string _deviceName)
    {
       
        if (_deviceName == "")
        {
            
            bHasMic = false;
        }
        else
        {
            if (Client.instance.username == "bob")
            {
                Debug.Log("Bob");
                bHasMic = false;
            }
            else
            {
                microphoneDevice = _deviceName;
                bHasMic = true;
                SetupMic();
            }
        }
        return true;
    }

    /// <summary>
    /// Sets chatterWebcamHandle
    /// </summary>
    /// <param name="handle"></param>
    public void SetChatterWebcamHandle(ChatterWebcamHandle handle)
    {
        chatterWebcamHandle = handle;
        audioSource = chatterWebcamHandle.GetComponent<AudioSource>();
        
    }

    public void SetupMic()
    {

        mic = Microphone.Start(microphoneDevice, true, 10, micFrequency);
        
        while (Microphone.GetPosition(microphoneDevice) < 0) { }

        //audioSource.clip = AudioClip.Create("test", 10 * micFrequency, mic.channels, micFrequency, false);
        //audioSource.loop = true;

        //audioSource.clip = Microphone.Start(microphoneDevice, true, 10, micFrequency);
        //audioSource.loop = true;
        //while (!(Microphone.GetPosition(microphoneDevice) > 2)){ }
        //audioSource.Play();
        
    }

    public void EnableDisableWebcam(bool bEnable)
    {
        if (bHasWebcam)
        {
            if (bEnable == true)
            {
                if (webCamTexture != null)
                {

                    chatterWebcamHandle.webcamRawImage.texture = webCamTexture;
                    chatterWebcamHandle.webcamRawImage.material.mainTexture = webCamTexture;
                    webCamTexture.Play();
                    bWebcamEnabled = true;
                }
                ClientSend.SendEnabledWebcam(true);
            }
            else
            {
                bWebcamEnabled = false;
                if (webCamTexture != null)
                {
                    if (webCamTexture.isPlaying)
                    {
                        webCamTexture.Stop();
                        chatterWebcamHandle.webcamRawImage.texture = GuiManager.instance.webcamDisabledTexture;
                        chatterWebcamHandle.webcamRawImage.material.mainTexture = GuiManager.instance.webcamDisabledTexture;
                    }
                }

                ClientSend.SendEnabledWebcam(false);
            }
        }
    }

    public void MuteMicrophone(bool bMute)
    {
        if (bHasMic)
        {
            if (bMute)
            {
                bMicMuted = true;
                ClientSend.SendMutedMic(bMute);
                //audioSource.Stop();
            }
            else
            {
                bMicMuted = false;
                ClientSend.SendMutedMic(bMute);
            }
        }
    }


    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    SendWebcamFrame();
        //}
        // send webcam frame 10 times a second

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (bSpacebarRoutineRunning)
            {
                StopCoroutine(spaceBarUnmuteDelay());
            }
            StartCoroutine(spaceBarUnmuteDelay());
        }    

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (bHoldingSpacebar)
            {
                //update button appearance
                GuiManager.instance.muteButton.GetComponent<Image>().sprite = GuiManager.instance.mutedIcon;
                MuteMicrophone(true);
                GridManager.instance.chatterWebcamHandles[Client.instance.myId].UpdateMuteIcon(true);
            }
            bHoldingSpacebar = false;
            if (bSpacebarRoutineRunning)
            {
                StopCoroutine(spaceBarUnmuteDelay());
            }
        }

        
    }


    IEnumerator spaceBarUnmuteDelay()
    {
        bSpacebarRoutineRunning = true;
        yield return new WaitForSeconds(.3f);
        if (Input.GetKey(KeyCode.Space))
        {
            bHoldingSpacebar = true;
            //update button appearance
            GuiManager.instance.muteButton.GetComponent<Image>().sprite = GuiManager.instance.unmutedIcon;
            MuteMicrophone(false);
            GridManager.instance.chatterWebcamHandles[Client.instance.myId].UpdateMuteIcon(false);
        }
        bSpacebarRoutineRunning = false;
    }

    private void FixedUpdate()
    {
        if (bHasMic)
        {
            try
            {
                micLatencyTimer += Time.fixedDeltaTime;
                if (micLatencyTimer >= latency)
                {
                    micLatencyTimer = 0f;
                    //get mic audio
                    //https://forum.unity.com/threads/microphone-network-test.123776/
                    int pos = Microphone.GetPosition(microphoneDevice);
                    int diff = pos - lastSample;
                    if (diff > 0)
                    {
                        if (!bMicMuted)
                        {
                            float[] samples = new float[diff * mic.channels];
                            mic.GetData(samples, lastSample);
                            //float[] newSamples = DownSampleAudio(samples, mic);
                            byte[] b = ToByteArray(samples);
                            //send mic audio to server as byte array
                            ClientSend.SendWebcamAudio(b.Length, b, mic.channels, micFrequency);

                        }


                    }
                    lastSample = pos;
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Issue processing mic data: {_ex}");
            }
        }
        if (bHasWebcam)
        {
            try
            {
                webcamLatencyTimer += Time.fixedDeltaTime;
                webcamFramerateTimer += Time.fixedDeltaTime;


                if (bWebcamEnabled)
                {

                    SendWebcamFrame();

                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Issue processing webcam data: {_ex}");
            }
            
        }
    }

    /// <summary>
    /// Called when game window changes focus
    /// </summary>
    /// <param name="focus"></param>
    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            //mute if we're holding the space bar when we click on another application
            if (bHoldingSpacebar)
            {
                //update button appearance
                GuiManager.instance.muteButton.GetComponent<Image>().sprite = GuiManager.instance.mutedIcon;
                MuteMicrophone(true);
                GridManager.instance.chatterWebcamHandles[Client.instance.myId].UpdateMuteIcon(true);
            }
            bHoldingSpacebar = false;
            if (bSpacebarRoutineRunning)
            {
                StopCoroutine(spaceBarUnmuteDelay());
            }
        }
    }

    public float[] DownSampleAudio(float[] _buffer,AudioClip _mic)
    {
        float modifier = (float)micFrequency / (float)modifiedFrequency;
        float length = _mic.length;
        int newNumberOfNewSamples = Mathf.FloorToInt(length * modifiedFrequency);
        int newBuffLen = newNumberOfNewSamples * _mic.channels;
        float[] newBuffer = new float[newBuffLen];
        for (int i = 0; i < newBuffLen; i++)
        {
            newBuffer[i] = _buffer[Mathf.FloorToInt(i * modifier)];
        }
        return newBuffer;
    }

    public void PlayAudio(int _chatterId,byte[] _byteAudio,int _micChannels,int _sampleRate)
    {
        
        float[] floatAudio = ToFloatArray(_byteAudio);
        if (GridManager.instance.chatterWebcamHandles.ContainsKey(_chatterId))
        {
            ChatterWebcamHandle chatterWebcamHandle = GridManager.instance.chatterWebcamHandles[_chatterId];
            AudioSource src = chatterWebcamHandle.GetComponent<AudioSource>();
            src.clip = AudioClip.Create("test", floatAudio.Length, _micChannels, _sampleRate, false);
            src.clip.SetData(floatAudio, 0);
            if (!src.isPlaying) src.Play();
        }
        
    }

    //https://stackoverflow.com/questions/4635769/how-do-i-convert-an-array-of-floats-to-a-byte-and-back
    public byte[] ToByteArray(float[] floatArray)
    {
        
        byte[] byteArray = new byte[floatArray.Length * sizeof(float)];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }
    //https://stackoverflow.com/questions/4635769/how-do-i-convert-an-array-of-floats-to-a-byte-and-back
    public float[] ToFloatArray(byte[] byteArray)
    {
       
        float[] floatArray = new float[byteArray.Length / sizeof(float)];
        Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
        return floatArray;
    }

    private void OnDestroy()
    {
        Microphone.End(null);
    }

    /// <summary>
    /// Send the typed message to the client.
    /// </summary>
    public void SendMessageToChat()
    {
        if (messageInputField.text.Length > 0 && !string.IsNullOrWhiteSpace(messageInputField.text))
        {
            if (Client.instance.SendMessageToChat(messageInputField.text))
            {
                messageInputField.text = "";
            }
        }
    }

    /// <summary>
    /// Add message to the chat.
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_message"></param>
    /// <param name="_chatter"></param>
    public void AddMessageToChatPanel(int _id, string _message, ChatterManager _chatter)
    {
        GameObject _messageObject = Instantiate(messageObject, chatPanel.transform);
        string _chatMessage;
        if (_id > 0)
        {

            _chatMessage = $"<b>{_chatter.username}</b>: {_message}";

            
            
        }
        else
        {
            _chatMessage = $"<b>SERVER</b>: {_message}";
            _messageObject.GetComponent<Text>().color = new Color(0.46f, 0f, 0f);
        }
        
        //limit how many messages are in the chat
        if (messageGameobjects.Count >= maxMessages)
        {

            Destroy(messageGameobjects[0].gameObject);
            messageGameobjects.RemoveAt(0);

        }
        _messageObject.GetComponent<Text>().text = _chatMessage;
        messageGameobjects.Add(_messageObject);

    }


    public void AddServerAnnouncement(string announcement)
    {
        serverAnnouncementText.text = $"<b>Server:</b> {announcement}";
        StopAllCoroutines();
        StartCoroutine(ServerAnnouncementRoutine());
    }

    private IEnumerator ServerAnnouncementRoutine()
    {
        yield return new WaitForSeconds(5f);
        serverAnnouncementText.text = "";
    }

    /// <summary>
    /// Tell client that we want to leave the chat
    /// </summary>
    public void ClientLeaveChat()
    {
        Client.instance.LeaveChat();
    }

    /// <summary>
    /// Relist the usernames of the chatters in the lobby
    /// </summary>
    public void UpdateLobbyPanel()
    {
        foreach  (Transform child in lobbyPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (ChatterManager chatter in AppManager.chatters.Values)
        {
            GameObject _lobbyTextObject = Instantiate(lobbyTextObject, lobbyPanel.transform);
            _lobbyTextObject.GetComponent<Text>().text = chatter.username;
        }
    }

    public void SendWebcamFrame()
    {
        
        
            
        //if we have enough stored frames to match the latency
        if (webcamLatencyBuffer.Count >= Mathf.CeilToInt(webcamFramerate * latency * 2.3f))
        {
            byte[] frameToSend = webcamLatencyBuffer[0];
            //send the byte array to the server
            ClientSend.SendWebcamFrame(frameToSend.Length, frameToSend);

            webcamLatencyBuffer.RemoveAt(0);
        }
        

        
        
        if (webCamTexture != null)
        {
            //create a texture2D
            Texture2D _tex = new Texture2D(webCamTexture.width, webCamTexture.height);
            //set the texture2D pixels to be the webcamTexture's pixels
            _tex.SetPixels32(webCamTexture.GetPixels32());
            //scale down the texture2D for performance reasons
            TextureScale.Bilinear(_tex, 100, 75);
            //get the texture2D as a byte array
            byte[] texBytes = _tex.GetRawTextureData();

            //save the frame data to the webcam latency buffer
            webcamLatencyBuffer.Add(texBytes);


            

        }
    }
}
