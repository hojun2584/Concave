using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;
using UnityEngine;
using Assets.Network.Packet;
using System.Linq;
using Unity.VisualScripting;

namespace DummyClient
{


	public class ServerSession : Session
	{
		public int sessionID;

		public Dictionary<ushort, Packet> packetDict = new Dictionary<ushort, Packet>();
		MakeSton stonePacket = new MakeSton();
		InitPacket InitPacket = new InitPacket();
		NextTurnPacket nextPacket = new NextTurnPacket();

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

		}

		public override void OnDisconnected(EndPoint endPoint)
		{
		}

		public override int OnRecv(ArraySegment<byte> buffer)
		{
            int pos = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            pos += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
            pos += 2;
			

			switch ((PacketID)id)
			{
				case PacketID.MakeStone:
					{

						GameManager instance = GameManager.instance;

						stonePacket.Read(buffer);

						JobQueue.Instance.jobActions.Enqueue( () => {
                            GroundManager.instance.currentGround = GroundManager.instance.GetGround(stonePacket.x , stonePacket.y);
							GroundManager.instance.currentGround.IsWhite = stonePacket.isWhite;
							
                        });
					}
					break;
				case PacketID.InitPacket:
					{

						InitPacket.Read(buffer);

						JobQueue.Instance.jobActions.Enqueue(() => 
						{
							NetWorkObj.instance.session.sessionID = InitPacket.playerData.sessionId;
                            GameManager.instance.PlayerData = InitPacket.playerData;
                        });

					}
					break;

				case PacketID.NextTurn:
					{
						nextPacket.Read(buffer);

                        JobQueue.Instance.jobActions.Enqueue(() =>
                        {
                            GameManager.instance.IsMyTurn = NetWorkObj.instance.session.sessionID == nextPacket.sessionId;
                        });


                    }
					break;
				default:
					break;
			}


            return buffer.Count;
		}

		public override void OnSend(int numOfBytes)
		{
			Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}

}
