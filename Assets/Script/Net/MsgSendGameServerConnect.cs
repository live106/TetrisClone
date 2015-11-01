using UnityEngine;
using System.Collections;
using Net.Thrift;
using Thrift.Transport;
using Thrift.Protocol;

public class MsgSendGameServerConnect : MsgSendBase
{
    ReuqestGameConnect m_request = new ReuqestGameConnect();

    public override void Send()
    {
        Debug.Log("Send Game Server Connect");
        TMemoryBuffer trans = new TMemoryBuffer();
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        m_request.Gameserver = GameMsgStaticData.Instance().ServerName;
        m_request.Passport = GameMsgStaticData.Instance().Passport;
        m_request.Uid = GameMsgStaticData.Instance().UID;
        m_request.SequenceId = GameMsgStaticData.Instance().GetSequenceId();
        m_request.RandomKey = GameMsgStaticData.Encrypt(
            m_request.Gameserver
            + m_request.Uid.ToString()
            + m_request.SequenceId.ToString(), GameMsgStaticData.Instance().SecureKey);
        Debug.Log("m_request.Gameserver = " + m_request.Gameserver);

        Debug.Log("m_request.RandomKey = " + m_request.RandomKey);

        Debug.Log("m_request.Passport = " + m_request.Passport);

        Debug.Log("m_request.Uid = " + m_request.Uid);

        Debug.Log("m_request.SequenceId = " + m_request.SequenceId);
        m_request.Write(proto);
        SendMsg(PeerType.PEER_TYPE_GAME,
            Common.GetHash(typeof(ReuqestGameConnect).Name),
            trans.GetBuffer());
    }
}
