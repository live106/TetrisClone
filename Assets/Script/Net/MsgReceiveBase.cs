using UnityEngine;
using System.Collections;

public class MsgReceiveBase
{
    public virtual void ParsePacket(byte[] bytes)
    {
        Debug.LogError("消息解析没有重写，ID = " + this.GetType().Name);
    }

    public virtual void DoMsg()
    {
        Debug.LogError("消息处理没有重写，ID = " + this.GetType().Name);
    }
}
