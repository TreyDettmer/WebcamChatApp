using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GuiManager : MonoBehaviour
{

    public static GuiManager instance;
    public Button muteButton;
    public Button webcamButton;
    public Sprite unmutedIcon;
    public Sprite mutedIcon;
    public Sprite webcamEnabledIcon;
    public Sprite webcamDisabledIcon;
    public Texture webcamDisabledTexture;

    [Header("Input Options")]
    public GameObject micOptionsWindow = null;
    public GameObject micOptionsContent = null;
    public GameObject webcamOptionsWindow = null;
    public GameObject webcamOptionsContent = null;
    public GameObject micOptionPrefab = null;
    public GameObject webcamOptionPrefab = null;

    private bool bMuted = true;
    private bool bWebcamDisabled = true;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MuteMicrophone()
    {
        if (MainManager.instance.bHasMic)
        {
            if (webcamOptionsWindow.activeSelf == false && micOptionsWindow.activeSelf == false)
            {
                if (!MainManager.instance.bHoldingSpacebar)
                {
                    bMuted = !bMuted;
                    if (bMuted)
                    {
                        //update button appearance
                        muteButton.GetComponent<Image>().sprite = mutedIcon;
                        //tell main
                        MainManager.instance.MuteMicrophone(true);
                        GridManager.instance.chatterWebcamHandles[Client.instance.myId].UpdateMuteIcon(true);
                    }
                    else
                    {
                        //update button appearance
                        muteButton.GetComponent<Image>().sprite = unmutedIcon;
                        //tell main
                        MainManager.instance.MuteMicrophone(false);
                        GridManager.instance.chatterWebcamHandles[Client.instance.myId].UpdateMuteIcon(false);
                    }
                }
            }
        }

    }

    public void EnableDisableWebcam()
    {
        if (MainManager.instance.bHasWebcam)
        {
            if (webcamOptionsWindow.activeSelf == false && micOptionsWindow.activeSelf == false)
            {
                bWebcamDisabled = !bWebcamDisabled;
                if (bWebcamDisabled)
                {
                    //update button appearance
                    webcamButton.GetComponent<Image>().sprite = webcamDisabledIcon;
                    //tell main
                    MainManager.instance.EnableDisableWebcam(false);

                }
                else
                {
                    //update button appearance
                    webcamButton.GetComponent<Image>().sprite = webcamEnabledIcon;
                    //tell main
                    MainManager.instance.EnableDisableWebcam(true);
                }
            }
        }
    }

    public void EnableWebcamOptionsWindow(WebCamDevice[] _devices)
    {
        webcamOptionsWindow.SetActive(true);
        for (int i = 0; i < _devices.Length; i++)
        {
            // this is necessary since we can't pass i into the delegate function because i will be different by the time the function is called
            int tempI = i;

            GameObject webcamOption = Instantiate(webcamOptionPrefab, webcamOptionsContent.transform);
            webcamOption.GetComponentInChildren<TextMeshProUGUI>().text = _devices[tempI].name;
            webcamOption.GetComponent<Button>().onClick.AddListener(delegate { WebcamOptionSelected(_devices[tempI].name); });
        }
        GameObject noWebcamOption = Instantiate(webcamOptionPrefab, webcamOptionsContent.transform);
        noWebcamOption.GetComponentInChildren<TextMeshProUGUI>().text = "Continue without webcam";
        noWebcamOption.GetComponent<Button>().onClick.AddListener(delegate { WebcamOptionSelected(""); });
        GameObject refreshWebcamOption = Instantiate(webcamOptionPrefab, webcamOptionsContent.transform);
        refreshWebcamOption.GetComponentInChildren<TextMeshProUGUI>().text = "Refresh options";
        refreshWebcamOption.GetComponent<Button>().onClick.AddListener(delegate { RefreshWebcamOptions(); });
    }

    public void EnableMicOptionsWindow(string[] _devices)
    {
        micOptionsWindow.SetActive(true);
        for (int i = 0; i < _devices.Length; i++)
        {
            // this is necessary since we can't pass i into the delegate function because i will be different by the time the function is called
            int tempI = i; 

            GameObject micOption = Instantiate(micOptionPrefab, micOptionsContent.transform);
            micOption.GetComponentInChildren<TextMeshProUGUI>().text = _devices[tempI].ToString();
            micOption.GetComponent<Button>().onClick.AddListener(delegate { MicOptionSelected(_devices[tempI].ToString()); });
        }
        GameObject noMicOption = Instantiate(micOptionPrefab, micOptionsContent.transform);
        noMicOption.GetComponentInChildren<TextMeshProUGUI>().text = "Continue without mic";
        noMicOption.GetComponent<Button>().onClick.AddListener(delegate { MicOptionSelected(""); });
        GameObject refreshMicOption = Instantiate(micOptionPrefab, micOptionsContent.transform);
        refreshMicOption.GetComponentInChildren<TextMeshProUGUI>().text = "Refresh options";
        refreshMicOption.GetComponent<Button>().onClick.AddListener(delegate { RefreshMicOptions(); });
    }

    public void MicOptionSelected(string _micName)
    {
        if (MainManager.instance.ChangeMicDevice(_micName))
        {
            DisableMicOptionsWindow();
        }
        
    }

    public void WebcamOptionSelected(string _webcamName)
    {

        if (MainManager.instance.ChangeWebcamDevice(_webcamName))
        {
            DisableWebcamOptionsWindow();
        }
        
        
    }

    public void DisableWebcamOptionsWindow()
    {
        webcamOptionsWindow.SetActive(false);
    }

    public void ShowSettingsWindows()
    {
        
        foreach (Transform child in webcamOptionsContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in micOptionsContent.transform)
        {
            Destroy(child.gameObject);
        }
        EnableMicOptionsWindow(Microphone.devices);
        EnableWebcamOptionsWindow(WebCamTexture.devices);
    }

    public void DisableMicOptionsWindow()
    {
        micOptionsWindow.SetActive(false);
    }

    public void RefreshWebcamOptions()
    {
        //clear content
        foreach (Transform child in webcamOptionsContent.transform)
        {
            Destroy(child.gameObject);
        }
        EnableWebcamOptionsWindow(WebCamTexture.devices);
    }

    public void RefreshMicOptions()
    {
        //clear content
        foreach (Transform child in micOptionsContent.transform)
        {
            Destroy(child.gameObject);
        }
        EnableMicOptionsWindow(Microphone.devices);
    }

    
}
