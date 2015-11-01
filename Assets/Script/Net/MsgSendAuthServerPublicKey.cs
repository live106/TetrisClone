using UnityEngine;
using System.Collections;
using Net.Thrift;
using Thrift.Transport;
using Thrift.Protocol;

public class MsgSendAuthServerPublicKey : MsgSendBase 
{
    RequestAuthServerPublicKey m_request = new RequestAuthServerPublicKey();
	
    public override void Send()
    {
        Debug.Log("RequestAuthServerPublicKey");
        TMemoryBuffer trans = new TMemoryBuffer();
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        m_request.Write(proto);
        SendMsg(PeerType.PEER_TYPE_ACCOUNT, 
            Common.GetHash(typeof(RequestAuthServerPublicKey).Name),
            trans.GetBuffer());
    }
}
