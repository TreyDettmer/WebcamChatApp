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

    public WebCamTexture webCamTexture;
    private float lastSentTime = 0f;
    private Texture2D webCamTexture2D;

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


    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    SendWebcamFrame();
        //}
        // send webcam frame 20 times a second
        if (Time.time - lastSentTime >= .1f)
        {
            SendWebcamFrame();
        }
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
            
            Texture2D _tex = new Texture2D(webCamTexture.width, webCamTexture.height);
            //Debug.Log($"Width: {webCamTexture.width}, Height: {webCamTexture.height}, w2: {webCamTexture.requestedWidth}, w3: {webCamTexture.requestedHeight}");
            _tex.SetPixels32(webCamTexture.GetPixels32());
            //Debug.Log($"Width: {webCamTexture.width} height: {webCamTexture.height}");
            TextureScale.Bilinear(_tex, 100, 75);
            
            //Color[] pixels = _tex.GetPixels();
            //int[] grayPixels = new int[pixels.Length];
            //for (int i = 0; i < pixels.Length; i++)
            //{
            //    grayPixels[i] = (int)(pixels[i].grayscale * 255f);
            //}
            //byte[] grayBytes = new byte[grayPixels.Length * sizeof(int)];
            //Buffer.BlockCopy(grayPixels, 0, grayBytes, 0, grayBytes.Length);
            byte[] texBytes = _tex.GetRawTextureData();
            //Debug.Log($"Texbytes: {texBytes.Length}");
            ClientSend.SendWebcamFrame(texBytes.Length, texBytes);
        }
    }
}
