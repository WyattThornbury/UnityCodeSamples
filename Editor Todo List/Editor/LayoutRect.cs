using System;
using UnityEditor;
using UnityEngine;

public class LayoutRect
{
    //rect .x never changes
    private Rect _rect = new Rect();
    private float _x = 0;
    private int _indentLevel = 0;
    //add support for margin left
    //add support for dynamic margins
    private Vector2 _margin = new Vector2(0, 10);

    public float x => _rect.x;

    public float y => _rect.y;

    public float IndentAmount { get; set; } = 20;
    public int IndentLevel
    {
        get { return _indentLevel; }
        set
        {
            if (_x == _Indent)
            {
                _indentLevel = value;
                _x = _Indent;
            }
            else
            {
                _indentLevel = value;
            }
        }
    }
    private bool _OnNewLine => _RemainingWidth == _LineWidth;
    private float _RemainingWidth => _rect.width - _x;
    private float _LineWidth => _rect.width - _Indent;

   
    private float _Indent => (IndentAmount * IndentLevel);

    public LayoutRect(float width)
    {
        _rect.width = width - _margin.y;
        _rect.height = EditorGUIUtility.singleLineHeight;
    }

    public LayoutRect(float width, float xMargin)
    {
        _rect.width = width - _margin.y;
        _margin.x = xMargin;
        _x = xMargin;
        _rect.height = EditorGUIUtility.singleLineHeight;
    }

    public Rect LineRect()
    {
        var tempRect = BuildRect(_Indent + _margin.x, _LineWidth);
        NextLine();
        return tempRect;
    }

    public void NextLine(bool updateUnityGUI = false)
    {
        
        _rect.y += EditorGUIUtility.singleLineHeight;
        _rect.y += EditorGUIUtility.standardVerticalSpacing;
        _x = _Indent + _margin.x;
        if (updateUnityGUI)
        {
            GUILayoutUtility.GetRect(_rect.width, _rect.height + EditorGUIUtility.standardVerticalSpacing);
        }
    }

    public Rect GetRectOfWidth(float amount)
    {
        var tempRect = BuildRect(_x, amount);
        _x += amount;
        return tempRect;
    }

    public void AddLineHeight(float multiplier)
    {
        var height = (EditorGUIUtility.singleLineHeight * multiplier);
        _rect.y += height;
        GUILayoutUtility.GetRect(_rect.width, height);
    }

    public Rect GetRectOfPercent(float percent)
    {
        var requestedWidth = _rect.width * percent;
        var tempRect = BuildRect(_x, requestedWidth);
        _x += requestedWidth;
        return tempRect;
    }

    public Rect GetRectFromEnd(float amount)
    {
        var startPoint = _LineWidth - amount;
        var tempRect = BuildRect(startPoint, amount);
        _x = startPoint;
        return tempRect;
    }

    public Rect GetRectUntilRemainingWidth(float remaining)
    {
        var width = _RemainingWidth - remaining;
        var tempRect = BuildRect(_x, width);
        _x += width;
        return tempRect;
    }

    public Rect GetRemainingWidth(bool updateUnity = false)
    {
        var tempRect = BuildRect(_x, _RemainingWidth);
        _x += _RemainingWidth;
        NextLine(updateUnity);
        return tempRect;
    }

    public Rect GetPercentOfRemainingWidth(float percentRemaining)
    {
        var width = _RemainingWidth * percentRemaining;
        var tempRect = BuildRect(_x, width);
        _x += width;
        return tempRect;
    }

    private Rect BuildRect(float x, float width)
    {
        return new Rect(x, _rect.y, width, _rect.height);
    }

    public float GetFlexWidth(float widthUsed)
    {
        return _LineWidth - widthUsed;
    }

    public void AddBufferOfWidth(float width)
    {
        _x += width;
    }

    public void UpdateFromLayoutRect(Rect rect) 
    {
        _rect.y = rect.y + rect.height;
        _rect.y += EditorGUIUtility.standardVerticalSpacing;
        _rect.x = _Indent;

    }

    public void UpdateLayoutRect()
    {
        GUILayoutUtility.GetRect(_rect.width, _rect.height);
    }


    public void Reset()
    {
        if (!_OnNewLine)
        {
            NextLine();
        }
        IndentLevel = 0;
    }

    public override string ToString()
    {
        return $"x {_rect.x} y {_rect.y} width {_rect.width} height {_rect.height}";
    }
}