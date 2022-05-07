using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ValueControll : MonoBehaviour
{
    public bool isToggle = false;
    public Action<bool,ValueControll> UpDownAction;
    public Action<ValueControll> ValueChangeAction;

    Button Up;
    Button Down;
    InputField InputValue;
    Toggle toggleValue;

    public bool isSet = false;
    
    private void Awake()
    {
        if (!isSet)
            UiSet();
    }

    void UiSet()
    {
        if (isToggle)
        {
            toggleValue = transform.Find("Toggle").GetComponent<Toggle>();
            toggleValue.onValueChanged.AddListener(delegate { ValueChangeAction(this); });
        }
        else
        {
            InputValue = transform.Find("InputField").GetComponent<InputField>();
            InputValue.onEndEdit.AddListener(delegate { ValueChangeAction(this); });
            Up = transform.Find("Up").GetComponent<Button>();
            Up.onClick.AddListener(delegate { UpDownAction(true, this); });
            Down = transform.Find("Down").GetComponent<Button>();
            Down.onClick.AddListener(delegate { UpDownAction(false, this); });
        }
        isSet = true;
    }


    public void SetValue(string _value)
    {
        if (!isSet)
            UiSet();
        if (isToggle)
        {
            return;
        }
        else
        {            
            InputValue.text = _value;
            InputValue.onEndEdit.Invoke("");
        }
    }

    public void SetValue(bool _value)
    {
        if (!isSet)
            UiSet();
        if (isToggle)
        {
            toggleValue.isOn = _value;
        }
        else
        {
            return;
        }
    }

    public string GetValue()
    {
        if (isToggle)
            return toggleValue.isOn.ToString();
        else
            return InputValue.text;
    }
}
