using UnityEngine;
using System.Collections;
using Thrift.Transport;
using Thrift.Protocol;
using Net.Thrift;

public class MsgReceiveAuthServerPublicKey : MsgReceiveBase 
{
    ResponseAuthServerPublicKey m_response = new ResponseAuthServerPublicKey();

    static public void RegisterMessage()
    {
        NetWorkMgr.Instance().RegisterMessage(Common.GetHash(typeof(ResponseAuthServerPublicKey).Name), typeof(MsgReceiveAuthServerPublicKey));
    }

    public override void ParsePacket(byte[] bytes)
    {
        Debug.Log("receive ResponseAuthServerPublicKey");
        TMemoryBuffer trans = new TMemoryBuffer(bytes);
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        m_response.Read(proto);
    }

    public override void DoMsg()
    {
        GameMsgStaticData.ServerPublicKey = m_response.ServerPubKey;
        ClientLoginMgr.Instance().SendClientPublicKey();
    }
}
