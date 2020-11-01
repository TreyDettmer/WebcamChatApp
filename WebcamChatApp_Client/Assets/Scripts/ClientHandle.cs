using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
/// <summary>
/// Handles data sent from server
/// </summary>
public class ClientHandle : MonoBehaviour
{
    /// <summary>
    /// Initial welcome when first connected to server
    /// </summary>
    /// <param name="_packet"></param>
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _id = _packet.ReadInt();

        
        Client.instance.myId = _id;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    /// <summary>
    /// Add a chatter to the cjat
    /// </summary>
    /// <param name="_packet"></param>
    public static void AddChatter(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        AppManager.instance.AddChatter(_id, _username);

    }

    /// <summary>
    /// Add a message to the chat
    /// </summary>
    /// <param name="_packet"></param>
    public static void SendMessage(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _message = _packet.ReadString();
        ChatterManager _chatter = AppManager.chatters[_id];
        if (_chatter != null)
        {
            if (MainManager.instance != null)
            {
                MainManager.instance.AddMessageToChatPanel(_id, _message, _chatter);
            }
        }
    }

    /// <summary>
    /// Remove the chatter from the lobby since they disconnected
    /// </summary>
    /// <param name="_packet"></param>
    public static void ChatterDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        AppManager.instance.RemoveChatter(_id);
        //if (MainManager.instance != null)
        //{
        //    AppManager.instance.RemoveChatter(_id);
        //    //MainManager.instance.UpdateLobbyPanel();
        //}
       
    }

    /// <summary>
    /// Add a message from the server to the chat
    /// </summary>
    /// <param name="_packet"></param>
    public static void ServerChatMessage(Packet _packet)
    {
        
        string _message = _packet.ReadString();
        if (MainManager.instance != null)
        {
            MainManager.instance.AddServerAnnouncement(_message);
        }
    }

    public static void UDPTest(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log($"Recieved packet via UDP. Contains message: {_msg}");
        ClientSend.UDPTestReceived();
    }

    public static void ChatterWebcamFrame(Packet _packet)
    {
        try
        {
            int _clientId = _packet.ReadInt();
            int _frameLength = _packet.ReadInt();
            byte[] _frame = _packet.ReadBytes(_frameLength);

            if (GridManager.instance != null)
            {
                GridManager.instance.UpdateChatterWebcam(_clientId, _frame);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error handling received texture data: {_ex}");
        }
    }

    public static void ChatterWebcamAudio(Packet _packet)
    {
        try
        {
            int _clientId = _packet.ReadInt();
            int _micChannels = _packet.ReadInt();
            int _sampleRate = _packet.ReadInt();
            int _audioLength = _packet.ReadInt();
            byte[] _audio = _packet.ReadBytes(_audioLength);
            MainManager.instance.PlayAudio(_clientId, _audio,_micChannels,_sampleRate);


        }
        catch (Exception _ex)
        {
            Debug.Log($"Error handling received audio data: {_ex}");
        }
    }

    public static void ChatterEnabledWebcam(Packet _packet)
    {
        int chatterId = _packet.ReadInt();
        bool enabled = _packet.ReadBool();
        GridManager.instance.ChatterEnabledWebcam(chatterId, enabled);
    }

    public static void ChatterMutedMic(Packet _packet)
    {
        int chatterId = _packet.ReadInt();
        bool muted = _packet.ReadBool();
        GridManager.instance.ChatterMutedMic(chatterId, muted);

    }
}
