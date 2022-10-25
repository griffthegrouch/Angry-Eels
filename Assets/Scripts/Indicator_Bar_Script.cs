using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]


public class Indicator_Bar_Script : MonoBehaviour//         7/312020
{
    // used to display a charge/counter amount on the screen
    // HOWTO: parent the charge bar object to the camera and position/orient, scale, colour how desired,
    // everytime a value you wish to display in the indicator bar changes, call this script's binding method, UpdateIndicator()

    // modular (adjust size/position/colour)
    public Color ChargeColour = Color.green;
    Color _chargeColour;

    public bool Auto_Set_Background_Colour;
    public Color BackGroundColour = Color.grey;
    Color _backGroundColour;

    public bool Auto_Set_Target_Colour;
    public Color TargetColour = Color.white;
    Color _targetColour;

    public bool Display_Text;

    public float _maxSize;
    float _size;
    float _value;
    float _percentValue;

    Transform[] children;
    Transform background;   // full size dark background of the charge bar
    Transform target;    // the small portion of the charge bar used to indicate the "double tap" window, or the tiny bit of the bar, thats hard to time
    Transform chargeBar;    // the moving part of the charge bar

    TextMesh text;    // the text that can display the percentage of the charge bar 

    private void OnValidate()//called when variables are changed in editor
    {
        // grabs all sprites, and depending on the options selected, colours them based off the charge bar colour 
        children = GetComponentsInChildren<Transform>();
        background = children[2];
        chargeBar = children[3];
        text = children[4].GetComponent<TextMesh>();
        target = children[5];
        background.transform.localScale = new Vector2(background.transform.localScale.x, _maxSize);

        if (Auto_Set_Background_Colour)
        {
            Color colour = Color.white;
            colour.r = ChargeColour.r * .35f;
            colour.g = ChargeColour.g * .35f;
            colour.b = ChargeColour.b * .35f;
            BackGroundColour = colour;
        }

        if (Auto_Set_Target_Colour)
        {
            Color colour = Color.white;
            colour.r = ChargeColour.r * 1.95f;
            colour.g = ChargeColour.g * 1.95f;
            colour.b = ChargeColour.b * 1.95f;
            TargetColour = colour;
        }
        if (_chargeColour != ChargeColour)
        {
            _chargeColour = ChargeColour;
            chargeBar.GetComponent<SpriteRenderer>().color = _chargeColour;
        }
        if (_backGroundColour != BackGroundColour)
        {
            _backGroundColour = BackGroundColour;
            background.GetComponent<SpriteRenderer>().color = _backGroundColour;
        }
        if (_targetColour != TargetColour)
        {
            _targetColour = TargetColour;
            target.GetComponent<SpriteRenderer>().color = _targetColour;
        }
    }

        public void UpdateIndicator(float size, float value)
    {   //manually call to update the indicator bar- everytime the counter value changes, call with max bar value, and the current bar value or "count"
        //eg. 1/4 full bar could be UpdateIndicator(100, 25);

        _value = value;

        if (_size != size)
        {
            _size = size;
            _percentValue = value / size;
        }

        chargeBar.localScale = new Vector2(chargeBar.localScale.x, _percentValue * _maxSize);

        if (Display_Text)
        {
            text.text = ((_percentValue * 100).ToString() + "%");
        }
        else
        {
            text.text = "";
        }
        
    }
}
