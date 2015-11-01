using UnityEngine;
using System.Collections;
using Thrift.Transport;
using Thrift.Protocol;
using Net.Thrift;

public class MsgReceiveGameServerConnect : MsgReceiveBase
{
    ResponseGameConnect m_response = new ResponseGameConnect();

    static public void RegisterMessage()
    {
        NetWorkMgr.Instance().RegisterMessage(Common.GetHash(typeof(ResponseGameConnect).Name), typeof(MsgReceiveGameServerConnect));
    }

    public override void ParsePacket(byte[] bytes)
    {
        Debug.Log("receive ResponseGameConnect");
        TMemoryBuffer trans = new TMemoryBuffer(bytes);
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        m_response.Read(proto);
    }

    public override void DoMsg()
    {
        Debug.Log("DoMsg ServerKey = " + m_response.Msg);
    }
}
