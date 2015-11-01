using UnityEngine;
using System.Collections;
using Net.Thrift;
using Thrift.Transport;
using Thrift.Protocol;
using System.Security.Cryptography;

public class MsgSendClientPublicKey : MsgSendBase
{
    RequestSendClientPublicKey m_request = new RequestSendClientPublicKey();

    public override void Send()
    {
        Debug.Log("RequestSendClientPublicKey");
        TMemoryBuffer trans = new TMemoryBuffer();
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        m_request.ClientPubKey = GameMsgStaticData.ClientKey;
        m_request.Write(proto);
        SendMsg(PeerType.PEER_TYPE_ACCOUNT,
            Common.GetHash(typeof(RequestSendClientPublicKey).Name),
            trans.GetBuffer());
    }
}
