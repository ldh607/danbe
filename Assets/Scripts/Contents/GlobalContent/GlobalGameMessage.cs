using UnityEngine;
using CellBig.Constants;
using System.Collections.Generic;

namespace CellBig.Contents.Event
{
    // 게임 공통 이벤트
    public class GameObjectDeActiveMessage : Message // 콘텐츠에서 오브젝트가 삭제될떄 풀에서도 삭제하기위한 메세지입니다.
    {
        public int TypeIndex = 0;
        public int OtherInfo = 0;
        public int AnotherInfo = 0;
        public GameObject myObject;

        public GameObjectDeActiveMessage(GameObject myObject)
        {
            this.myObject = myObject;
        }

        public GameObjectDeActiveMessage(int typeIndex, GameObject myObject)
        {
            this.myObject = myObject;
            TypeIndex = typeIndex;
        }

        public GameObjectDeActiveMessage(int typeIndex = 0, int otherInfo = 0, GameObject myObject = null)
        {
            this.myObject = myObject;
            TypeIndex = typeIndex;
            OtherInfo = otherInfo;
        }

        public GameObjectDeActiveMessage(int typeIndex = 0, int otherInfo = 0, int anotherInfo = 0, GameObject myObject = null)
        {
            this.myObject = myObject;
            TypeIndex = typeIndex;
            OtherInfo = otherInfo;
            AnotherInfo = anotherInfo;
        }
    }

    public class GameObjectHitMessage : Message // 판타게임에서 터치되면 호출되는 메세지 입니다.
    {
        public GameObject myObject;

        public GameObjectHitMessage(GameObject myObject)
        {
            this.myObject = myObject;
        }
    }

    public class PoolObjectMsg : Message { }

    public class MultiTouchMsg : Message { }

    public class TouchRectMsg : Message
    {
        public List<UnityEngine.Rect> TouchRects;

        public TouchRectMsg(List<UnityEngine.Rect> touchRects)
        {
            TouchRects = touchRects;
        }
    }

    public class GenericComponent<T> : Message
    {
        public T[] Component;

        public GenericComponent(T[] component)
        {
            Component = component;
        }
    }

    public class MainCameraMsg : Message
    {
        public Camera MainCamera;
        public MainCameraMsg(Camera camera)
        {
            MainCamera = camera;
        }
    }

    public class ShakeCameraMsg : Message
    {
        public float Duration;
        public float Amount;
        public bool IsX;
        public bool IsY;
        public ShakeCameraMsg(float duration, float amount, bool isX = true, bool isY = true)
        {
            Duration = duration;
            Amount = amount;
            IsX = isX;
            IsY = isY;
        }
    }

    public class ColorCameraMsg : Message
    {
        public float Duration;
        public Color Color;
        public ColorCameraMsg(float duration, Color color)
        {
            Duration = duration;
            Color = color;
        }
    }

}
