using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using Tangatek.AITrees;

namespace Tangatek.AITreesEditor
{
    /// <summary>
    /// Canopy editor base, use CanopyEditor instead
    /// </summary>
    public class CanopyInspectorBase
    {
        #region Properties
        private Node _target;
        /// <summary>
        /// The Node being inspected
        /// </summary>
        public Node target { get { return _target; } }
        protected AITree tree;
        #endregion

        #region Draw
        public static void Draw(CanopyInspectorBase inspector)
        {
            if (inspector == null) return;
            inspector.DrawBaseInspector();
            inspector.OnInspectorGUI();
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates the editor using the type provided
        /// </summary>
        public static CanopyInspectorBase CreateEditor(AITree tree, Node obj, Type editorType)
        {
            if (obj == null) return null;
            var editor = (CanopyInspectorBase)Activator.CreateInstance(editorType);
            editor._target = obj;
            editor.tree = tree;
            return editor;
        }
        public void SetProperties(AITree tree, Node obj)
        {
            _target = obj;
            this.tree = tree;
        }
        #endregion

        #region Editor
        /// <summary>
        /// Draw the GUI for user to interact with
        /// </summary>
        public virtual void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
        #endregion

        #region Drawing
        private void DrawBaseInspector()
        {
            //	Show name (always)
            EditorGUILayout.BeginHorizontal();
            target.name = EditorGUILayout.TextField("Name:", target.name);

            if (GetTypeDescription(target.GetType()).Length > 0)
                if (GUILayout.Button(new GUIContent("?", GetTypeDescription(target.GetType())), GUILayout.Width(20)))
                    EditorUtility.DisplayDialog(target.GetType().Name, GetTypeDescription(target.GetType()), "OK");
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Draws the default inspector.
        /// </summary>
        protected void DrawDefaultInspector()
        {
            DrawDefaultPropertiesExcluding(target, "");
        }


        /// <summary>
        /// Draws the default properties excluding the given list
        /// </summary>
        protected internal void DrawDefaultPropertiesExcluding(Node obj, params string[] propertyToExclude)
        {

            //	Create explusions list (Add defaults to it)
            List<string> exclusions = new List<string>(propertyToExclude);
            exclusions.AddRange(new string[] { "id", "childId", "childrenIds", "_name", "serializedCanopy" });
            //	Draw each property with default means
            foreach (var field in obj.GetType().GetFields())
            {
                if (field.GetCustomAttributes(typeof(HideInInspector), true) != null) continue;
                if (exclusions.Contains(field.Name)) continue;
                field.SetValue(target, DrawDefaultProperty(field.GetValue(target), field.Name, field.FieldType));
            }
                    
        }


        /// <summary>
        /// Draws a property with default gui.
        /// </summary>
        protected internal static System.Object DrawDefaultProperty(System.Object obj, string title, Type objType)
        {
            //  title formatting
            title = Regex.Replace(title, "(\\B[A-Z])", " $1");
            title = char.ToUpper(title[0]) + title.Substring(1);

            if (obj is int) return EditorGUILayout.IntField(title, (int)obj);
            else if (obj is float) return EditorGUILayout.FloatField(title, (float)obj);
            else if (obj is bool) return EditorGUILayout.Toggle(title, (bool)obj);
            else if (obj is string) return EditorGUILayout.TextField(title, (string)obj);
            else if (obj is Color) return EditorGUILayout.ColorField(title, (Color)obj);
            else if (obj is AnimationCurve) return EditorGUILayout.CurveField(title, (AnimationCurve)obj);
            else if (obj is Bounds) return EditorGUILayout.BoundsField(title, (Bounds)obj);
            else if (obj is Rect) return EditorGUILayout.RectField(title, (Rect)obj);
            else if (obj is Vector2) return EditorGUILayout.Vector2Field(title, (Vector2)obj);
            else if (obj is Vector3) return EditorGUILayout.Vector3Field(title, (Vector3)obj);
            else if (obj is Vector4) return EditorGUILayout.Vector4Field(title, (Vector4)obj);
            else if (obj is Enum) return EditorGUILayout.EnumPopup(title, (Enum)obj);
            else if (obj is LayerMask) return EditorGUILayout.MaskField(title, (LayerMask)obj, LayerMaskNames);
            else if (typeof(UnityEngine.Object).IsAssignableFrom(objType)) return EditorGUILayout.ObjectField(title, (UnityEngine.Object)obj, objType, true);

            Debug.Log("Unkown type:" + obj.GetType());

            return obj;

        }

        #endregion

        #region Helpers
        public static string GetTypeDescription(Type type)
        {
            var attrib = (Description)Attribute.GetCustomAttribute(type, typeof(Description));
            if (attrib == null) return "";
            return attrib.text;
        }
        private static string[] LayerMaskNames
        {
            get
            {
                List<string> names = new List<string>();
                for (var i = 0; i < 32; i++)
                    if (LayerMask.LayerToName(i).Length > 0)
                        names.Add(LayerMask.LayerToName(i));
                return names.ToArray();
            }
        }
        #endregion
    }
}
