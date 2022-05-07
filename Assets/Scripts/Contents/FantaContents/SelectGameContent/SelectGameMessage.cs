using System.Collections.Generic;
using CellBig.Contents;
using CellBig.Constants;
using System;
using UnityEngine;


namespace CellBig.UI.Event
{


    public class SelectContentMsg : Message
    {
        public ContentType contentType;
        public List<GameData> contentsDatas;
        public SelectContentMsg(ContentType contentType, List<GameData> contentsDatas)
        {
            this.contentType = contentType;
            this.contentsDatas = contentsDatas;
        }
    };
}
