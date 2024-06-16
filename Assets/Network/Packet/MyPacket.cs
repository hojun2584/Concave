using ServerCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Network.Packet
{

    public enum PacketID
    {
        InitPacket = 1,
        PlayerInfoOk = 2,
        playerPos = 3,
        RpcPacket = 4,
        MakeStone = 5
    }

    public abstract class Packet
    {

        public ushort size;
        public ushort packetId;

        public abstract void Read(ArraySegment<byte> buffer);
        public abstract ArraySegment<byte> Write();

        public void WriteString(ref ArraySegment<byte> buffer , ref ushort size ,string value)
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
        public void WriteVector3(ref ArraySegment<byte> buffer, ref ushort size, Vector3 pos)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset, buffer.Count), pos.X);
            size += sizeof(float);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset, buffer.Count), pos.Y);
            size += sizeof(float);
            BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset, buffer.Count), pos.Z);
            size += sizeof(float);
        }

        public Vector3 ReadVector3(ref ArraySegment<byte> buffer, ref ushort size)
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


    public class InitPacket : Packet
    {

        public bool isMaster = false;
        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            //packetId 처리 안할꺼라서 bitconverter사용안함
            count += sizeof(ushort);
            count += sizeof(ushort);

            isMaster = BitConverter.ToBoolean(buffer.Array, buffer.Offset + count);
            count += sizeof(Boolean);
        }

        public override ArraySegment<byte> Write()
        {
            ushort count = 0;
            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.MakeStone);
            count += sizeof(ushort);


            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> doneBuffer = SendBufferHelper.Close(count);

            return doneBuffer;
        }
        public ArraySegment<byte> Write(bool isMaster)
        {
            ushort count = 0;
            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.MakeStone);
            count += sizeof(ushort);

            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), isMaster);
            count += sizeof(Boolean);

            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> doneBuffer = SendBufferHelper.Close(count);

            return doneBuffer;
        }
    }


    public class MakeSton : Packet
    {
        public ushort x, y;
        public bool color = false;


        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ReadOnlySpan<byte> readBuffer = new ReadOnlySpan<byte>(buffer.Array,buffer.Offset,buffer.Count);
            //packetId 처리 안할꺼라서 bitconverter사용안함
            count += sizeof(ushort);


            count += sizeof(ushort);
            x = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);

            count += sizeof(ushort);
            y = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);


        }

        public override ArraySegment<byte> Write()
        {
            ushort count = 0;
            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            //todo 에러 메시지라도 보낼 것


            ArraySegment<byte> doneBuffer = SendBufferHelper.Close(count);

            return doneBuffer;
        }

        public ArraySegment<byte> Write(ushort x , ushort y)
        {
            ushort count = 0;

            ArraySegment<byte> sendBuffer = SendBufferHelper.Open(4096);
            count += sizeof(ushort);
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)PacketID.MakeStone);
            count += sizeof(ushort);

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
            BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset + count, sendBuffer.Count - count), (ushort)4);
            count += sizeof(ushort);

            WriteString(ref sendBuffer,ref count, value);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(new Span<byte>(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count), count);
            ArraySegment<byte> endBuffer = SendBufferHelper.Close(count);

            return endBuffer;
        }

    }


    public class MakeWhiteRock : Packet
    {

        bool success = true;

        public override void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            double x, y, z;
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
