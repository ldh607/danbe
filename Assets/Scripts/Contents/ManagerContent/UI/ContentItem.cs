using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig;
using CellBig.Models;

public class ContentItem : MonoBehaviour
{
    Text Title;
    Image Icon;
    Button Up;
    Button Down;
    Toggle Check;

    int itemIndex = -1 ;
    bool isSet = false;

    private void Awake()
    {
        if (!isSet)
            UiSet();
    }

    void UiSet()
    {
        if( Check == null )
        {
            Check = this.transform.Find("Check").GetComponent<Toggle>();
            Check.onValueChanged.AddListener(delegate { ToggleChange(); });
        }

        if (Title == null)
        {
            Title = this.transform.Find("Title").GetComponent<Text>();
        }
        if (Icon == null)
        {
            Icon = this.transform.Find("Icon").GetComponent<Image>();
        }

        if (Up == null)
        {
            Up = transform.Find("Up").GetComponent<Button>();
            Up.onClick.AddListener(UpButtonDown);
        }
        if (Down == null)
        {
            Down = transform.Find("Down").GetComponent<Button>();
            Down.onClick.AddListener(DownButtonDown);
        }
        isSet = true;
    }

    public void SetContent( string name, Sprite icon , bool isUse , int index )
    {
        if (!isSet)
            UiSet();

        itemIndex = index;
        Title.text = name;
        Check.isOn = isUse;
        Icon.sprite = icon;        
    }
    
    public void SetIndex(int index )
    {
        itemIndex = index;
    }

    public void SetCheck(bool value)
    {
        Check.isOn = value;
    }

    public void ToggleChange()
    {
        Message.Send<CellBig.UI.Event.ContentToggleChange>(new CellBig.UI.Event.ContentToggleChange(itemIndex, Check.isOn ));
    }

    public void UpButtonDown()
    {
        if (itemIndex < 0)
            return;
        Message.Send<CellBig.UI.Event.ContentListChange>(new CellBig.UI.Event.ContentListChange( itemIndex , itemIndex-1));
    }

    public void DownButtonDown()
    {
        if (itemIndex < 0)
            return;
        Message.Send<CellBig.UI.Event.ContentListChange>(new CellBig.UI.Event.ContentListChange(itemIndex, itemIndex +1));
    }
}
