using UnityEngine;
using System;
using System.Collections;
using Net.Thrift;
using Thrift.Transport;
using Thrift.Protocol;

public class MsgSendBase
{
	public virtual void Send()
    {
        Debug.LogError("msg send not rewrite");
    }

    protected void SendMsg(PeerType targetType, int nMsgHashCode,byte[] bodyBytes)
    {
        int nTotalLength = 0;
        int nHeaderLenth = 0;
        int nBodyLength = 0;
        TMemoryBuffer HeaderBuffer = new TMemoryBuffer();
        TBinaryProtocol HeaderProto = new TBinaryProtocol(HeaderBuffer);
        ProtocolHeader msgThriftHead = new ProtocolHeader();
        msgThriftHead.TargetType = targetType;
        msgThriftHead.SourceType = PeerType.PEER_TYPE_CLIENT;
        msgThriftHead.ProtocolHash = nMsgHashCode;
        msgThriftHead.SerializeType = SerializeType.SERIALIZE_TYPE_THRIFT;
        msgThriftHead.Write(HeaderProto);
        byte[] HeaderBytes = HeaderBuffer.GetBuffer();
        nHeaderLenth = HeaderBytes.Length;
        nBodyLength = bodyBytes.Length;
        nTotalLength = sizeof(Int32) + sizeof(Int32) + nHeaderLenth + sizeof(Int32) + nBodyLength;
        byte[] MsgBuffer = new byte[nTotalLength];
        int nCursor = 0;
        byte[] totalLengthBytes = BitConverter.GetBytes(nTotalLength);
        Array.Reverse(totalLengthBytes);
        Array.Copy(totalLengthBytes, 0, MsgBuffer, nCursor, sizeof(Int32));
        nCursor += sizeof(Int32);
        byte[] headerLengthBytes = BitConverter.GetBytes(nHeaderLenth);
        Array.Reverse(headerLengthBytes);
        Array.Copy(headerLengthBytes, 0, MsgBuffer, nCursor, sizeof(Int32));
        nCursor += sizeof(Int32);
        Array.Copy(HeaderBytes, 0, MsgBuffer, nCursor, HeaderBytes.Length);
        nCursor += HeaderBytes.Length;
        byte[] bodyLengthBytes = BitConverter.GetBytes(nBodyLength);
        Array.Reverse(bodyLengthBytes);
        Array.Copy(bodyLengthBytes, 0, MsgBuffer, nCursor, sizeof(Int32));
        nCursor += sizeof(Int32);
        Array.Copy(bodyBytes, 0, MsgBuffer, nCursor, bodyBytes.Length);

        NetWorkMgr.Instance().SendMsg(MsgBuffer);
    }
}
