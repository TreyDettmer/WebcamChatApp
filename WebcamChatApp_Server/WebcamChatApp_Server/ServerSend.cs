using System;
using System.Collections.Generic;
using System.Text;

namespace WebcamChatApp_Server
{
    class ServerSend
    {
        public static void Welcome(int _toClient, string _str)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_str);
                _packet.Write(_toClient);
                SendTCPData(_toClient, _packet);

            }
        }

        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);

        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }

        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }

        }

        private static void SendUDPPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }

        }

        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }

        }

        #region Packets

        public static void UDPTest(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.udpTest))
            {
                _packet.Write("A test packet for UDP!");
                SendUDPData(_toClient, _packet);
            }
        }

        public static void AddChatter(int _toClient, Chatter _chatter)
        {
            using (Packet _packet = new Packet((int)ServerPackets.addChatter))
            {
                _packet.Write(_chatter.id);
                _packet.Write(_chatter.username);

                SendTCPData(_toClient, _packet);

            }
        }

        public static void ServerChatMessage(string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverChatMessage))
            {
                _packet.Write(_msg);

                SendTCPDataToAll(_packet);
            }
        }

        public static void ServerChatMessage(int _exceptClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverChatMessage))
            {
                _packet.Write(_msg);

                SendTCPDataToAll(_exceptClient,_packet);
            }
        }

        public static void ChatterDisconnected(int _clientId)
        {
            using (Packet _packet = new Packet((int)ServerPackets.chatterDisconnected))
            {
                _packet.Write(_clientId);

                SendTCPDataToAll(_packet);
            }
        }

        public static void SendChatterWebcamFrame(int _exceptClient,int _frameLength, byte[] _webcamFrame)
        {
            
            using (Packet _packet = new Packet((int)ServerPackets.chatterWebcamFrame))
            {
                _packet.Write(_exceptClient);
                _packet.Write(_frameLength);
                _packet.Write(_webcamFrame);
                SendUDPDataToAll(_exceptClient, _packet);
            }
        }

        public static void SendChatterWebcamAudio(int _exceptClient,int _audioLength, byte[] _webcamAudio,int _micChannels,int _sampleRate)
        {
            using (Packet _packet = new Packet((int)ServerPackets.chatterWebcamAudio))
            {
                _packet.Write(_exceptClient);
                _packet.Write(_micChannels);
                _packet.Write(_sampleRate);
                _packet.Write(_audioLength);
                _packet.Write(_webcamAudio);
                SendUDPDataToAll(_exceptClient, _packet);
            }
        }

        public static void SendChatterWebcamEnabled(int _exceptClient,bool _enabled)
        {
            using (Packet _packet = new Packet((int)ServerPackets.chatterEnabledWebcam))
            {
                _packet.Write(_exceptClient);
                _packet.Write(_enabled);
                SendTCPDataToAll(_exceptClient, _packet);
            }
        }

        public static void SendChatterMicMuted(int _exceptClient, bool _muted)
        {
            using (Packet _packet = new Packet((int)ServerPackets.chatterMutedMic))
            {
                _packet.Write(_exceptClient);
                _packet.Write(_muted);
                SendTCPDataToAll(_exceptClient, _packet);
            }
        }

        #endregion

    }
}
