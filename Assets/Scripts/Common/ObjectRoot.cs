using UnityEngine;
using System.Collections;
using CellBig.Contents.Event;
using System.Collections.Generic;

/// <summary>
/// 오브젝트의 공통 부모
/// Object_Mng하고 같이 사용할때 상속받아야할 클래스
/// </summary>
public class Object_Root : MonoBehaviour
{
    internal bool bActive = false;
    /// <summary>
    /// 오브젝트 Pool에 만들어 질때 호출되서 기본 초기화 및 메모리 잡을 때 이 함수 안에서 사용
    /// </summary>
    protected virtual void Awake()
    {

    }

    /// <summary>
    /// 최종적으로 오브젝트 Pool에서까지 죽을때 호출됨 마지막 메모리 해제 할 때 이 함수 안에서 사용
    /// </summary>
    protected virtual void OnDestroy()
    {

    }

    /// <summary>
    /// 오브젝트가 Pool에서 실제로 사용될때 호출되는 함수
    /// </summary>
    virtual public void Active()
    {
       // Message.AddListener<HitCheckMsg>(OnCheckHit);

        bActive = true;
    }

    /// <summary>
    /// m_bActive가 false되면 Object_Mng에서 자동으로 오브젝트 Pool에 되돌려줌
    /// </summary>
    virtual public void DeActive()
    {
        bActive = false;
      //  Message.Send<CellBig.Contents.Event.GameObjectDeActiveMessage>(new CellBig.Contents.Event.GameObjectDeActiveMessage(this.gameObject)); // 죽음 메세지
    }

     public virtual void Hit()
    {
     //   Message.Send<CellBig.Contents.Event.GameObjectHitMessage>(new CellBig.Contents.Event.GameObjectHitMessage(this.gameObject)); // 히트 메세지
    }

    public bool GetActive()
    {
        return bActive;
    }

    virtual public void Update_Obj()
    {

    }

}
