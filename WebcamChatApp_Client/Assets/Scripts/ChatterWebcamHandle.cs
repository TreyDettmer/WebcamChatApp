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
        mutedIcon.enabled = !bMuted;
    }
}
