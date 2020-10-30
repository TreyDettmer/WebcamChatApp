using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// Handles sending data to the server.
/// </summary>
public class ClientSend : MonoBehaviour
{
    /// <summary>
    /// Sends TCP data to server
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets

    /// <summary>
    /// Send client info once we connect to server
    /// </summary>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(Client.instance.username);

            SendTCPData(_packet);
        }
    }


    public static void UDPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.udpTestReceived))
        {
            _packet.Write("Received a UDP packet.");

            SendUDPData(_packet);
        }
    }

    /// <summary>
    /// Sends webcam frame to server
    /// </summary>
    /// <param name="_length">length of byte array</param>
    /// <param name="_webcamFrame">frame as byte array</param>
    public static void SendWebcamFrame(int _length, byte[] _webcamFrame)
    {
        using (Packet _packet = new Packet((int)ClientPackets.webcamFrame))
        {
            _packet.Write(_length);
            _packet.Write(_webcamFrame);
            SendUDPData(_packet);
            
        }
    }



    #endregion


}
