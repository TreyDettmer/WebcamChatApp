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
    private float lastSentTime = 0f;
    private Texture2D webCamTexture2D;
    private AudioClip mic;
    public AudioSource audioSource;
    public int micFrequency = 44100;
    private int modifiedFrequency = 8000;
    private string microphoneDevice;

    bool bHasWebcam = false;
    bool bHasMic = false;
    bool bWebcamEnabled = false;
    bool bMicMuted= true;

    private int lastSample;





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
        WebCamDevice[] webcamDevices = WebCamTexture.devices;
        if (webcamDevices.Length == 0)
        {
            bHasWebcam = false;
        }
        else
        {
            bHasWebcam = true;
            webCamTexture = new WebCamTexture();
            webCamTexture.requestedHeight = 75;
            webCamTexture.requestedWidth = 100;
            webCamTexture.requestedFPS = 20f;
            webCamTexture.deviceName = webcamDevices[0].name;
            //Debug.Log($"Found webcam: {webcamDevices[0].name}");
        }
        if (Client.instance.username == "bob")
        {
            Debug.Log("Bob");
            bHasMic = false;
        }
        else
        {
            string[] microphoneDevices = Microphone.devices;
            if (microphoneDevices.Length == 0)
            {
                bHasMic = false;

            }
            else
            {
                bHasMic = true;
                //Debug.Log($"Found microphone: {microphoneDevices[0]}");
                microphoneDevice = microphoneDevices[0];
                SetupMic();

            }
        }
        

    }

    


    /// <summary>
    /// Change the webcam device
    /// </summary>
    /// <param name="_device">webcam device</param>
    private void ChangeWebcamDevice(WebCamDevice _device)
    {
        if (webCamTexture != null)
        {
            webCamTexture.deviceName = _device.name;
        }
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
        if (bHasWebcam)
        {
            if (bWebcamEnabled)
            {
                if (Time.time - lastSentTime >= .1f)
                {
                    SendWebcamFrame();
                }
            }
        }


        if (bHasMic)
        {
            //get mic audio
            //https://forum.unity.com/threads/microphone-network-test.123776/
            int pos = Microphone.GetPosition(microphoneDevice);
            int diff = pos - lastSample;
            if (diff > 0)
            {
                if (!bMicMuted)
                {
                    float[] samples = new float[diff * 1];
                    mic.GetData(samples, lastSample);
                    byte[] b = ToByteArray(samples);
                    //send mic audio to server as byte array
                    ClientSend.SendWebcamAudio(b.Length, b);
                    
                }


            }
            lastSample = pos;

            
        }
    }

    public void PlayAudio(int _chatterId,byte[] _byteAudio)
    {
        
        float[] floatAudio = ToFloatArray(_byteAudio);
        if (GridManager.instance.chatterWebcamHandles.ContainsKey(_chatterId))
        {
            ChatterWebcamHandle chatterWebcamHandle = GridManager.instance.chatterWebcamHandles[_chatterId];
            AudioSource src = chatterWebcamHandle.GetComponent<AudioSource>();
            src.clip = AudioClip.Create("test", floatAudio.Length, 1, micFrequency, false);
            src.clip.SetData(floatAudio, 0);
            if (!src.isPlaying) src.Play();
        }
        
    }

    //https://forum.unity.com/threads/microphone-network-test.123776/
    public byte[] ToByteArray(float[] floatArray)
    {
        int len = floatArray.Length * 4;
        byte[] byteArray = new byte[len];
        int pos = 0;
        foreach (float f in floatArray)
        {
            byte[] data = BitConverter.GetBytes(f);
            Array.Copy(data, 0, byteArray, pos, 4);
            pos += 4;
        }
        return byteArray;
    }
    //https://forum.unity.com/threads/microphone-network-test.123776/
    public float[] ToFloatArray(byte[] byteArray)
    {
        int len = byteArray.Length / 4;
        float[] floatArray = new float[len];
        for (int i = 0; i < byteArray.Length; i += 4)
        {
            floatArray[i / 4] = BitConverter.ToSingle(byteArray, i);
        }
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
        lastSentTime = Time.time;
        
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
            //send the byte array to the server
            ClientSend.SendWebcamFrame(texBytes.Length, texBytes);
        }
    }
}
