using UnityEngine;
using System.Collections;
using Net.Thrift;
using Thrift.Transport;
using Thrift.Protocol;

public class MsgReceiveUserLogin : MsgReceiveBase
{
    ResponseUserLogin m_response = new ResponseUserLogin();

    static public void RegisterMessage()
    {
        NetWorkMgr.Instance().RegisterMessage(Common.GetHash(typeof(ResponseUserLogin).Name), typeof(MsgReceiveUserLogin));
    }

    public override void ParsePacket(byte[] bytes)
    {
        Debug.Log("receive ResponseUserLogin");
        TMemoryBuffer trans = new TMemoryBuffer(bytes);
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        m_response.Read(proto);
    }

    public override void DoMsg()
    {
        GameMsgStaticData.Instance().ServerName = m_response.Gameserver;
        GameMsgStaticData.Instance().SecureKey = m_response.SecureKey;
        GameMsgStaticData.Instance().Passport = m_response.Passport;
        GameMsgStaticData.Instance().UID = m_response.Uid;
        Debug.Log("DoMsg ServerKey = " + m_response.Msg);

        MsgSendGameServerConnect msg = new MsgSendGameServerConnect();
        msg.Send();
    }
}