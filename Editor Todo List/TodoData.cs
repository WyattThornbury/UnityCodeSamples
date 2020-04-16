using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TodoData : ScriptableObject
{
    public List<TodoSlot> slots = null;

    public void AddTodo(TodoSlot dataHolder)
    {
        slots.Insert(0,dataHolder);
    }
}

[System.Serializable]
public class TodoSlot 
{
    public string text;
    public string title;
    public bool complete;
}
