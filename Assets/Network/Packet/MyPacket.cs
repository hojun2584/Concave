using ServerCore;
using System;
using System.Numerics;
using System.Text;

namespace Assets.Network.Packet
{

    public enum PacketID
    {
        InitPacket = 1,
        PlayerInfoOk = 2,
        PlayerPos = 3,
        RpcPacket = 4,
        MakeStone = 5,
        NextTurn = 10,
    }

    public abstract class Packet
    {

        public ushort size;
        public ushort packetId;

        public abstract void Read(ArraySegment<byte> buffer);
        public abstract ArraySegment<byte> Write();

        protected void WriteString(ref ArraySegment<byte> buffer , ref ushort size ,string value)
        {
            ushort stringLength = (ushort)Encoding.Unicode.GetByteCount(value);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + size, buffer.Count - size), (ushort)stringLength);
            size += sizeof(ushort);

            byte[] sendString = Encoding.Unicode.GetBytes(value);
            for (int i = 0; i < stringLength; i++)
            {
                buffer[i + size] = sendString[i];
            }
            size += stringLength;

        }
        protected void WriteVector3(ref ArraySegment<byte> buffer, ref ushort size, Vector3 pos)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset, buffer.Count), pos.X);
            size += sizeof(float);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset, buffer.Count), pos.Y);
            size += sizeof(float);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset, buffer.Count), pos.Z);
            size += sizeof(float);
        }
        protected Vector3 ReadVector3(ref ArraySegment<byte> buffer, ref ushort size)
        {
            Vector3 compleVector3 = new Vector3();

            compleVector3.X = BitConverter.ToSingle(buffer.Array, buffer.Offset + size);
            size += sizeof(float);
            compleVector3.Y = BitConverter.ToSingle(buffer.Array, buffer.Offset + size);
            size += sizeof(float);
            compleVector3.Z = BitConverter.ToSingle(buffer.Array, buffer.Offset + size);
            size += sizeof(float);

            return compleVector3;
        }


    }

    public class NextTurnPacket : Packet
    {
        public int sessionId;

        public void PacketInit(int sessionId)
        {
            this.sessionId = sessionId;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            count += sizeof(ushort);
            count += sizeof(ushort);

            sessionId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(int);

        }

        public override ArraySegment<byte> Write()
        {
            ushort count = 0;
            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.NextTurn);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), sessionId);
            count += sizeof(int);

            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            return SendBufferHelper.Close(count);
        }
    }

    public class InitPacket : Packet
    {

        public bool isMaster = false;

        public PlayerStruct playerData;

        public void PacketInit(PlayerStruct playerData)
        {
            this.playerData = playerData;
        }

        public void PacketInit(int sessionId, bool isPlayer, bool isWhite)
        {
            playerData.sessionId = sessionId;
            playerData.isWhite = isWhite;
            playerData.isPlayer = isPlayer;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            //packetId 처리 안할꺼라서 bitconverter사용안함
            count += sizeof(ushort);
            count += sizeof(ushort);

            playerData.sessionId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(int);

            playerData.isPlayer = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);

            playerData.isWhite = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);

        }


        public override ArraySegment<byte> Write()
        {
            ushort count = 0;
            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.MakeStone);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), playerData.sessionId);
            count += sizeof(int);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), playerData.isPlayer);
            count += sizeof(bool);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), playerData.isWhite);
            count += sizeof(bool);

            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> doneBuffer = SendBufferHelper.Close(count);

            return doneBuffer;
        }
    }


    public class MakeSton : Packet
    {
        public ushort x, y;
        public bool isWhite;


        public void InitPacket(ushort x , ushort y , bool isWhite)
        {
            this.isWhite = isWhite;
            this.x = x;
            this.y = y;
        }

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ReadOnlySpan<byte> readBuffer = new ReadOnlySpan<byte>(buffer.Array,buffer.Offset,buffer.Count);
            //packetId 처리 안할꺼라서 bitconverter사용안함
            count += sizeof(ushort);
            count += sizeof(ushort);

            isWhite = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(bool);
            
            x = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);
            y = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);
        }

        public override ArraySegment<byte> Write()
        {

            ushort count = 0;

            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.MakeStone);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), isWhite);
            count += sizeof(bool);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), x);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), y);
            count += sizeof(ushort);


            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> doneBuffer = SendBufferHelper.Close(count);
            return doneBuffer;
        }

    }


    public class RPCPacket : Packet
    {
        public string funcName = null;


        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            string readTest;
            ReadOnlySpan<byte> readBuffer = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count);
            count += sizeof(ushort);

            count += sizeof(ushort);
            ushort stringSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            Console.WriteLine($"stringSize : {stringSize}");
            count += 2;

            readTest = Encoding.Unicode.GetString(buffer.Array, count, stringSize);
            Console.WriteLine($"receivString : {readTest}");
            count += stringSize;
        }

        public override ArraySegment<byte> Write()
        {
            ushort count = 0;
            bool success = true;
            ushort test = 3;

            string value = "테스트용 string";

            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.RpcPacket );
            count += sizeof(ushort);

            WriteString(ref sendBuffer,ref count, value);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> endBuffer = SendBufferHelper.Close(count);

            return endBuffer;
        }


        public ArraySegment<byte> BroadCastWrite()
        {
            if(funcName == null)
            {
                Console.WriteLine("TODO using this func after Read();");

                return null;
            }

            ushort count = 0;
            bool success = true;

            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.RpcPacket );
            count += sizeof(ushort);

            WriteString(ref sendBuffer, ref count, funcName);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> endBuffer = SendBufferHelper.Close(count);

            return endBuffer;
        }
    }

    public class ResultPacket : Packet
    {
        public bool winner;

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ReadOnlySpan<byte> readBuffer = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count);
            count += sizeof(ushort);





        }

        public override ArraySegment<byte> Write()
        {
            ushort count = 0;
            return null;
        }
        
        public ArraySegment<byte> Write(bool winningPlayer)
        {

            ushort count = 0;

            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count +=sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array,sendBuffer.Offset+count , sendBuffer.Count - count) , winningPlayer);
            count += sizeof(bool);



            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset,sendBuffer.Count) , count);
            return SendBufferHelper.Close(count);
        }

    }
    public class MakeWhiteRock : Packet
    {

        bool success = true;

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ReadOnlySpan<byte> readBuffer = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count);
            //packetId
            Vector3 pos = new Vector3();
            count += sizeof(ushort);
            count += sizeof(ushort);
            //!! NOTICE read  읽은 뒤에 그만큼 count를 뒤로 밀기
            pos.X = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(double);
            pos.Y = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(double);
            pos.Z = BitConverter.ToSingle(buffer.Array, buffer.Offset + count);
            count += sizeof(double);
            Console.WriteLine($"x :{pos.X} y:{pos.Y} z:{pos.Z}");

        }

        public override ArraySegment<byte> Write()
        {
            ushort count = 0;

            double x = 0.5, y = 1.5, z = 2.5;

            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)5);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), x);
            count += sizeof(double);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), y);
            count += sizeof(double);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), z);
            count += sizeof(double);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> endBuffer = SendBufferHelper.Close(count);

            return endBuffer;
        }
    }
}
