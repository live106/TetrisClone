﻿using UnityEngine;
using System.Collections;
using Thrift.Transport;
using Thrift.Protocol;
using Net.Thrift;

public class MsgReceiveClientPublicKey : MsgReceiveBase
{
    ResponseSendClientPublicKey m_response = new ResponseSendClientPublicKey();

    static public void RegisterMessage()
    {
        NetWorkMgr.Instance().RegisterMessage(Common.GetHash(typeof(ResponseSendClientPublicKey).Name), typeof(MsgReceiveClientPublicKey));
    }

    public override void ParsePacket(byte[] bytes)
    {
        Debug.Log("receive ResponseSendClientPublicKey");
        TMemoryBuffer trans = new TMemoryBuffer(bytes);
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        m_response.Read(proto);
    }

    public override void DoMsg()
    {
        Debug.Log("DoMsg ServerKey = " + m_response.Msg);
        ClientLoginMgr.Instance().Login();
    }
}
