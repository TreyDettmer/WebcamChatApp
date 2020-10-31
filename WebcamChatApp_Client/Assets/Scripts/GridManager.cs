using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    //total background width & height: 1920 by 1080
    //grid elements should have aspect ratio of 4:3




    public Dictionary<int, ChatterWebcamHandle> chatterWebcamHandles = new Dictionary<int, ChatterWebcamHandle>();
    public GameObject chatterGridElement;


    public RawImage webcamImage;

    public static GridManager instance;


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

    // Start is called before the first frame update
    void Start()
    {
        //WebCamTexture webcamTexture = new WebCamTexture();
        //webcamImage.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 150);
        //webcamTexture.requestedWidth = 200;
        //webcamTexture.requestedHeight = 150;
        //webcamImage.texture = webcamTexture;
        //webcamImage.material.mainTexture = webcamTexture;
        //webcamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Adds a chatter's webcam to the grid
    /// </summary>
    /// <param name="_chatterId">chatter id</param>
    public void AddChatterGridElement(int _chatterId)
    {
        ChatterWebcamHandle chatterWebcamHandle = Instantiate(chatterGridElement, transform).GetComponent<ChatterWebcamHandle>();
        chatterWebcamHandle.id = _chatterId;
        chatterWebcamHandle.SetUsername(AppManager.chatters[_chatterId].username);
        chatterWebcamHandle.InstantiateMaterial();
        chatterWebcamHandle.webcamRawImage.texture = GuiManager.instance.webcamDisabledTexture;
        chatterWebcamHandle.webcamRawImage.material.mainTexture = GuiManager.instance.webcamDisabledTexture;
        chatterWebcamHandles.Add(_chatterId, chatterWebcamHandle);
        if (_chatterId == Client.instance.myId)
        {

            MainManager.instance.SetChatterWebcamHandle(chatterWebcamHandle);
        }
        Debug.Log("Added element to grid");
        GenerateGrid();
    }

    /// <summary>
    /// Removes a chatter's webcam from the grid
    /// </summary>
    /// <param name="_chatterId">id of chatter to remove</param>
    public void RemoveChatterGridElement(int _chatterId)
    {
        ChatterWebcamHandle handleToDestroy = chatterWebcamHandles[_chatterId];
        chatterWebcamHandles.Remove(_chatterId);
        Destroy(handleToDestroy.gameObject);
        GenerateGrid();
    }

    /// <summary>
    /// Generates the grid
    /// </summary>
    public void GenerateGrid()
    {
        foreach (ChatterWebcamHandle chatterWebcamHandle in chatterWebcamHandles.Values)
        {
            GameObject obj = chatterWebcamHandle.gameObject;
            foreach (Transform child in obj.transform)
            {
                RectTransform rect = child.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.ForceUpdateRectTransforms();
                }
            }

        }

    }

    public void UpdateChatterWebcam(int _chatterIndex, byte[] _frame)
    {
        if (chatterWebcamHandles.ContainsKey(_chatterIndex))
        {
            ChatterWebcamHandle chatterWebcamHandle = chatterWebcamHandles[_chatterIndex];

            chatterWebcamHandle.SetWebcamImage(_frame);

        }
    }

    public void ChatterEnabledWebcam(int _chatterIndex, bool _enabled)
    {
        if (chatterWebcamHandles.ContainsKey(_chatterIndex))
        {
            ChatterWebcamHandle chatterWebcamHandle = chatterWebcamHandles[_chatterIndex];
            if (_enabled == false)
            {
                chatterWebcamHandle.DisableWebcamImage();
            }

        }
    }

    public void ChatterMutedMic(int _chatterIndex, bool _muted)
    {
        if (chatterWebcamHandles.ContainsKey(_chatterIndex))
        {
            ChatterWebcamHandle chatterWebcamHandle = chatterWebcamHandles[_chatterIndex];
            chatterWebcamHandle.UpdateMuteIcon(_muted);
        }
    }
}
