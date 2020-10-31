using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChatterWebcamHandle : MonoBehaviour
{

    public int id = -1;
    public string username = "";
    public bool bMuted = false;
    public RawImage webcamRawImage;
    [HideInInspector] public Material webcamMaterial;

    [SerializeField] private TextMeshProUGUI usernameTMP;
    [SerializeField] private Image mutedIcon;
    [SerializeField] private Shader shader;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite unMutedSprite;


    [SerializeField] private RectTransform infoPanelRect;
    [SerializeField] private RectTransform userNameTMPRect;
    [SerializeField] private RectTransform mutedRect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateMaterial()
    {
        webcamMaterial = new Material(shader);
        webcamRawImage.material = webcamMaterial;

    }


    /// <summary>
    /// Sets username
    /// </summary>
    /// <param name="_username">chatter username</param>
    public void SetUsername(string _username)
    {
        username = _username;
        usernameTMP.text = username;
    }

    /// <summary>
    /// Sets muted state
    /// </summary>
    /// <param name="_bMuted">should this chatter be muted</param>
    public void SetMuted(bool _bMuted)
    {
        bMuted = _bMuted;
        
    }

    public void UpdateChildren()
    {
        //infoPanelRect.sizeDelta = new Vector2(webcamRawImage.rectTransform.rect.width, webcamRawImage.rectTransform.rect.height / 6f);
        //userNameTMPRect.anchoredPosition = new Vector2(20f, -62.5f);
        //userNameTMPRect.sizeDelta = new Vector2(infoPanelRect.rect.width * .9f, infoPanelRect.rect.height);
        //mutedRect.anchoredPosition = new Vector2(985, -62.5f);
        //mutedRect.sizeDelta = new Vector2(infoPanelRect.rect.height, infoPanelRect.rect.height);
        
    }

    public void SetWebcamImage(byte[] bytes)
    {
        try
        {
            Texture2D blank = new Texture2D(100, 75);
            blank.LoadRawTextureData(bytes);
            blank.Apply();
            TextureScale.Bilinear(blank, 320, 180);
            //Texture2D webCamTex2D = ToTexture2D(webcamRawImage.texture);
            
            //webCamTex2D.LoadRawTextureData(bytes);
            //webCamTex2D.Apply();
            webcamRawImage.texture = blank;
            //TextureScale.Bilinear(_tex, 320, 180);
            webcamRawImage.material.mainTexture = blank;
            
        }
        catch (Exception _ex)
        {
            Debug.Log($"Issue copying texture onto grid element: {_ex}");
        }
    }

    public void DisableWebcamImage()
    {
        webcamRawImage.texture = GuiManager.instance.webcamDisabledTexture;
        webcamRawImage.material.mainTexture = GuiManager.instance.webcamDisabledTexture;
    }

    public void UpdateMuteIcon(bool _mute)
    {
        if (_mute)
        {
            mutedIcon.sprite = mutedSprite;
        }
        else
        {
            mutedIcon.sprite = unMutedSprite;
        }
    }

    private Texture2D ToTexture2D(Texture tex)
    {
        try
        {
            Texture2D dest = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
            dest.Apply(false);
            Graphics.CopyTexture(tex, dest);
            return dest;
        }
        catch (Exception _ex)
        {
            Debug.Log($"Issue converting texture to texture2D: {_ex}");
            return null;
        }

    }
}
