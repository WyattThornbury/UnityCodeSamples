using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage(System.AttributeTargets.Method)]
public class EditorButtonSOAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableObject), true)]
public class EditorButtonSO : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var so = target as ScriptableObject;

        var methods = so.GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                        BindingFlags.NonPublic)
            .Where(o => Attribute.IsDefined(o, typeof(EditorButtonSOAttribute)));

        foreach (var memberInfo in methods)
        {
            if (GUILayout.Button(memberInfo.Name))
            {
                var method = memberInfo as MethodInfo;
                method.Invoke(so, null);
                EditorUtility.SetDirty(so);
            }
        }
    }
}
#endif