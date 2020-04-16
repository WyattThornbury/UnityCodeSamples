using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class TODO : EditorWindow 
{
    private TodoData data = null;
    private TodoSlot dataHolder = new TodoSlot();

    [MenuItem("TWT/TODO")]
    static void Init()
    {
        TODO window = (TODO)EditorWindow.GetWindow(typeof(TODO));
        window.Show();
        window.GetTodoData();
    }

    void OnGUI()
    {
        if (data == null) 
        {
            GetTodoData();
        }
        var rect = new LayoutRect(position.width, 4);
        rect.AddLineHeight(0.1f);

        DisplayTodos(rect);

        DisplayTodoCreation(rect);
    }

    private void DisplayTodos(LayoutRect rect)
    {
        bool useGrey = false;
        Color grey = new Color32(80, 80, 80, 255);
        Color green = new Color32(30, 118, 59, 255);


        for (int i = data.slots.Count - 1; i >= 0; i--)
        {
            var todoSlot = data.slots[i];

            if (useGrey && !todoSlot.complete)
            {
                GUI.DrawTexture(new Rect(0, rect.y, position.width, 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)),
                    EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, grey, 0, 0);
            }
            else if (todoSlot.complete) 
            {
                GUI.DrawTexture(new Rect(0, rect.y, position.width, 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)),
                    EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, green, 0, 0);
            }
            useGrey = !useGrey;

            EditorGUI.LabelField(rect.GetRectOfWidth(40f), "Todo:");
            EditorGUI.LabelField(rect.GetRectUntilRemainingWidth(80f), todoSlot.title, EditorStyles.boldLabel);
            EditorGUI.LabelField(rect.GetRectUntilRemainingWidth(10), "Complete:");
            EditorGUI.BeginChangeCheck();
            todoSlot.complete = EditorGUI.Toggle(rect.GetRemainingWidth(true), todoSlot.complete);
            if (EditorGUI.EndChangeCheck()) 
            {
                EditorUtility.SetDirty(data);
            }


            EditorGUI.LabelField(rect.GetRectUntilRemainingWidth(20), todoSlot.text);
            if(GUI.Button(rect.GetRemainingWidth(true), "x"))
            {
                data.slots.RemoveAt(i);
                EditorUtility.SetDirty(data);
            }
            rect.AddLineHeight(.2f);
        }
        rect.Reset();
    }

    private void DisplayTodoCreation(LayoutRect rect)
    {
        GUILayout.FlexibleSpace();
        rect.UpdateFromLayoutRect(GUILayoutUtility.GetLastRect());
        EditorGUI.LabelField(rect.GetRectOfWidth(40f), "Title: ");
        dataHolder.title = EditorGUI.TextField(rect.GetRemainingWidth(true), dataHolder.title);

        EditorGUI.LabelField(rect.GetRectOfWidth(40f), "Todo:");
        dataHolder.text = EditorGUI.TextArea(rect.GetRemainingWidth(true), dataHolder.text);

        if (GUI.Button(rect.GetRemainingWidth(true), "Create")) 
        {
            CreateTodo();
        }

        rect.AddLineHeight(.5f);
    }

    private void CreateTodo()
    {
        data.AddTodo(dataHolder);
        dataHolder = new TodoSlot();
    }

    private void GetTodoData()
    {
        
        data = AssetDatabaseUtil.GetAllAssetsOfType<TodoData>().FirstOrDefault();
        if (data == null)
        {
            TodoData temp = ScriptableObject.CreateInstance<TodoData>();
            AssetDatabaseUtil.CreateAsset(temp, "Assets/Scripts/Editor/TODO", "TODO Data");
            data = AssetDatabaseUtil.GetAllAssetsOfType<TodoData>().FirstOrDefault();
        }
    }
}
