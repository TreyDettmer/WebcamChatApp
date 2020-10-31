using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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

    public void EnableDisableWebcam()
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
