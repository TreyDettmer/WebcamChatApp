using System;
using System.Collections.Generic;
using System.Text;

namespace WebcamChatApp_Server
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientToCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_clientToCheck].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient} with username: {_username}");
            if (_fromClient != _clientToCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientToCheck})!");
            }
            Server.clients[_fromClient].SendIntoChat(_username);
        }

        public static void ChatterMessage(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Server.clients[_fromClient].chatter.SetMessage(_msg);
        }

        public static void UDPTestReceived(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine($"Recieved packet via UDP. Contains message: {_msg}");
        }

        public static void WebcamFrame(int _fromClient, Packet _packet)
        {
            int _frameLength = _packet.ReadInt();
            byte[] _frame = _packet.ReadBytes(_frameLength);
            ServerSend.SendChatterWebcamFrame(_fromClient, _frameLength, _frame);
        }

        public static void WebcamAudio(int _fromClient,Packet _packet)
        {
            int _micChannels = _packet.ReadInt();
            int _sampleRate = _packet.ReadInt();
            int _audioLength = _packet.ReadInt();
            byte[] _audio = _packet.ReadBytes(_audioLength);
            ServerSend.SendChatterWebcamAudio(_fromClient, _audioLength, _audio,_micChannels,_sampleRate);
        }

        public static void EnabledWebcam(int _fromClient, Packet _packet)
        {
            bool _enabled = _packet.ReadBool();
            ServerSend.SendChatterWebcamEnabled(_fromClient, _enabled);
        }

        public static void MutedMic(int _fromClient, Packet _packet)
        {
            bool _muted = _packet.ReadBool();
            ServerSend.SendChatterMicMuted(_fromClient, _muted);

        }

    }
}
