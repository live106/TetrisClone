using UnityEngine;
using System.Collections;
using Net.Thrift;
using Thrift.Transport;
using Thrift.Protocol;
using System.Security.Cryptography;
using System;

public class MsgSendUserLogin : MsgSendBase
{
    RequestUserLogin m_request = new RequestUserLogin();

//     public string m_strUserName;
//     public string m_strPassWor	
    public override void Send()
    {
        Debug.Log("RequestUserLogin");
        TMemoryBuffer trans = new TMemoryBuffer();
        TBinaryProtocol proto = new TBinaryProtocol(trans);
//         m_request.Username = GameStaticData.Encrypt(m_strUserName, GameStaticData.ClientKey);
//         m_request.Password = GameStaticData.Encrypt(m_strPassWord, GameStaticData.ClientKey);
//         m_request.SdkUid = "";
//         m_request.SdkToken = "";
        string randomKey = GameMsgStaticData.GetRandomKey(32);
        Debug.Log("randomKey = " + randomKey);
        m_request.MachineId = GameMsgStaticData.Encrypt(randomKey, GameMsgStaticData.ClientKey);
        Debug.Log("m_request.MachineId = " + m_request.MachineId.ToString());
        Debug.Log("GameStaticData.ClientKey = " + GameMsgStaticData.ClientKey.ToString());
        m_request.Type = LoginType.USER_GUEST;
        m_request.Write(proto);
        SendMsg(PeerType.PEER_TYPE_ACCOUNT,
            Common.GetHash(typeof(RequestUserLogin).Name),
            trans.GetBuffer());
    }
}
