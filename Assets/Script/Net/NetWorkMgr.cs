using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Net.Thrift;
using Thrift.Transport;
using Thrift.Protocol;
using System.Threading;

public class NetWorkMgr
{
    static NetWorkMgr m_instance;
    static public NetWorkMgr Instance()
    {
        if (m_instance == null)
        {
            m_instance = new NetWorkMgr();
            m_instance.Awake();
            m_instance.InitThread();
            m_instance.RegisterAllMessage();
        }
        return m_instance;
    }

    static public void DestoryIns()
    {
        m_instance.DestoryThread();
        m_instance = null;
    }

    string m_strHost;
    int m_nPort;

    private TcpClient m_tpcClient;
    Thread m_threadMsg;
    
    private byte[] m_buffer = new byte[8192];
    private byte[] m_tempBuffer = new byte[4096];

    int m_nMsgLength = 0;
    int m_nCurReadPos = 0;
    Queue<byte[]> m_queueSendBytes = new Queue<byte[]>();

    private Dictionary<int, Type> m_msgHash = new Dictionary<int, Type>();
    private Queue<MsgReceiveBase> m_queWillDo = new Queue<MsgReceiveBase>();

    bool m_bReceiveSocket = true;

    void Awake()
    {
        ResourceManager.Instance().Load("Tables/IPConfige", delegate(object parameter, ResourceManager.WWWResourceData rd)
        {
            if (rd != null && rd.text != null)
            {
                string[] strs = rd.text.Split('|');
                if (strs.Length == 2)
                {
                    m_strHost = strs[0];
                    m_nPort = int.Parse(strs[1]);
                }
            }
        }, null);
    }

    void InitThread()
    {
        m_threadMsg = new Thread(ReceiveSocket);
        m_threadMsg.Start();
    }

    void DestoryThread()
    {
        if (m_threadMsg != null)
        {
            m_bReceiveSocket = false;
            m_threadMsg.Abort();
            m_threadMsg = null;
        }
    }

    public void RegisterMessage(int msgHashKey, Type msgClass)
    {
        if (!m_msgHash.ContainsKey(msgHashKey))
        {
            m_msgHash.Add(msgHashKey, msgClass);
        }
        else
        {
            Debug.LogError("注册了相同的消息号!" + "MsgID = " + msgHashKey);
        }
    }

    public void RemoveMessage(int msgHashKey)
    {
        if (m_msgHash.ContainsKey(msgHashKey))
        {
            m_msgHash.Remove(msgHashKey);
        }
    }

    private void ReceiveSocket()
    {
        Debug.Log("ReceiveSocket");
        while (m_bReceiveSocket)
        {
            Debug.Log("ReceiveSocket while (true)");
            if (m_tpcClient != null && m_tpcClient.Connected)
            {
                try
                {
                    NetworkStream netWorkStream = m_tpcClient.GetStream();
                    if (netWorkStream.CanRead)
                    {
                        Debug.Log("ReceiveSocket recevie msg");

                        netWorkStream.BeginRead(m_tempBuffer, 0, m_tempBuffer.Length, new AsyncCallback(ReadStreamCallBack), netWorkStream);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
            Thread.Sleep(1000);
        }
    }

    private void ReadStreamCallBack(IAsyncResult ar)
    {
        Debug.Log("ReadStreamCallBack");
//         try
//         {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            Debug.Log("ns change");
            int nNumBytesRead = ns.EndRead(ar);
            Debug.Log("Read bytes num = " + nNumBytesRead.ToString());
            m_nCurReadPos += nNumBytesRead;
            Debug.Log("CurReadPos = " + m_nCurReadPos.ToString());
            if (m_nCurReadPos >= 4)
            {
                Array.Copy(m_tempBuffer, 0, m_buffer, m_nCurReadPos - nNumBytesRead, nNumBytesRead);
                if(m_nMsgLength == 0)
                {
                    Array.Reverse(m_buffer, 0, 4);
                    m_nMsgLength = BitConverter.ToInt32(m_buffer, 0);
                }
                if (m_nCurReadPos == m_nMsgLength)
                {
                    Debug.Log("begin ParsePacket m_nCurReadPos = " + m_nCurReadPos.ToString());
                    ParsePacket();
                }
                else
                {
                    Debug.Log("next BeginRead " + m_nCurReadPos.ToString());
                    ns.BeginRead(m_tempBuffer, 0, m_tempBuffer.Length, new AsyncCallback(ReadStreamCallBack), ns);
                }
            }
            else
            {
                Array.Copy(m_tempBuffer, 0, m_buffer, m_nCurReadPos - nNumBytesRead, nNumBytesRead);
                ns.BeginRead(m_tempBuffer, 0, m_tempBuffer.Length, new AsyncCallback(ReadStreamCallBack), ns);
            }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.Log("接收消息出错" + ex.Message + ex.StackTrace);
//         }
    }

    void ParsePacket()
    {
        int nCursor = 0;
        int nMsgTotalLength = BitConverter.ToInt32(m_buffer, nCursor);
        nCursor += sizeof(Int32);
        Array.Reverse(m_buffer, nCursor, sizeof(Int32));
        int nHeaderLength = BitConverter.ToInt32(m_buffer, nCursor);
        nCursor += sizeof(Int32);
        byte[] headerBytes = new byte[nHeaderLength];
        Array.Copy(m_buffer, nCursor, headerBytes, 0, nHeaderLength);
        nCursor += nHeaderLength;
        Array.Reverse(m_buffer, nCursor, sizeof(Int32));
        int nBodyLength = BitConverter.ToInt32(m_buffer, nCursor);
        nCursor += sizeof(Int32);
        byte[] bodyBytes = new byte[nBodyLength];
        Array.Copy(m_buffer, nCursor, bodyBytes, 0, nBodyLength);
        nCursor += nBodyLength;

        TMemoryBuffer trans = new TMemoryBuffer(headerBytes);
        TBinaryProtocol proto = new TBinaryProtocol(trans);
        ProtocolHeader msgThriftHead = new ProtocolHeader();
        msgThriftHead.Read(proto);
        if (m_msgHash.ContainsKey(msgThriftHead.ProtocolHash))
        {
            MsgReceiveBase msgRec = Activator.CreateInstance(m_msgHash[msgThriftHead.ProtocolHash]) as MsgReceiveBase;
            msgRec.ParsePacket(bodyBytes);
            m_queWillDo.Enqueue(msgRec);
        }
        m_nMsgLength = 0;
        m_nCurReadPos = 0;
//         CloseSocket();
    }

    void CloseSocket()
    {
        m_nMsgLength = 0;
        m_nCurReadPos = 0;
        m_tpcClient.Close();
        m_tpcClient = null;
    }

    public void Update()
    {
        while (m_queWillDo.Count > 0)
        {
            MsgReceiveBase mr = m_queWillDo.Dequeue();
            mr.DoMsg();
        }
    }

    private void ConnectToServer()
    {
        m_tpcClient = new TcpClient(AddressFamily.InterNetwork);
        m_tpcClient.BeginConnect(m_strHost, m_nPort, new AsyncCallback(ConnectCallback), m_tpcClient);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        if (m_tpcClient.Connected)
        {
            Debug.Log("has connected to server");
            if (m_queueSendBytes.Count > 0)
            {
                SendMsg(m_queueSendBytes.Dequeue());
            }
        }
    }

    public void SendMsg(byte[] bytes)
    {
        if (m_tpcClient == null)
        {
            ConnectToServer();
            m_queueSendBytes.Enqueue(bytes);
            return;
        }
        if (!m_tpcClient.Connected)
        {
            ConnectToServer();
            m_queueSendBytes.Enqueue(bytes);
            return;
        }
        if (bytes == null)
        {
            return;
        }

        NetworkStream networkStream = m_tpcClient.GetStream();
        if (networkStream != null)
            networkStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(WriteCallBack), networkStream);

    }

    private void WriteCallBack(IAsyncResult ar)
    {
        var ns = ar.AsyncState as NetworkStream;
        ns.EndWrite(ar);
    }

    public void RegisterAllMessage()
    {
        MsgReceiveAuthServerPublicKey.RegisterMessage();
        MsgReceiveClientPublicKey.RegisterMessage();
        MsgReceiveUserLogin.RegisterMessage();
        MsgReceiveGameServerConnect.RegisterMessage();
    }
}
