using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using Tangatek.AITrees;
using System.Reflection;
using System.Linq;
namespace Tangatek.AITreesEditor
{
    public abstract class TreeInspectorBase : Editor
    {
        #region Fields
        private AITree targetScript = null;
        #endregion

        #region Canopy
        private static AITree _SelectedTree = null;
        private static Canopy _SelectedCanopy = null;
        public static Canopy SelectedCanopy
        {
            get
            {
                if (Selection.activeGameObject == null) return null;
                var tree = Selection.activeGameObject.GetComponent<AITree>();
                if (tree == null) return null;
                if (tree != _SelectedTree) _SelectedCanopy = null;
                _SelectedTree = tree;
                if (_SelectedCanopy == null) _SelectedCanopy = Canopy.Create(tree.serializedCanopy, tree);
                return _SelectedCanopy;
            }
        }
        public static void ClearCanopy()
        {
            _SelectedCanopy = null;
        }
        #endregion

        #region TreeInspectorBase
        /// <summary>
        /// Gets list of inspector types that can render this object
        /// </summary>
        protected virtual List<Type> GetCanopyInspectorTypes() { return GetClassesWith(typeof(CanopyInspectorBase)); }
        protected virtual Type defaultInspectorType { get { return typeof(CanopyInspectorBase); } }
        protected virtual void OpenEditorWindow() { }
        #endregion

        #region Editor

        public override void OnInspectorGUI()
        {
            if(Application.isPlaying) return;
            targetScript = (AITree)target;
            if (GUILayout.Button("Open Editor Window")) OpenEditorWindow();
                
            //  Draw the properties of any custom tree
            DrawPropertiesExcluding(serializedObject, "serializedCanopy");
            DrawSeparator();

            if (SelectedCanopy == null) return;
 
            if (SelectedCanopy.selectedNode != null)
            {
                CanopyInspectorBase inspector = null;
                var inspectorType = FilterInspectorTypes(GetCanopyInspectorTypes(), SelectedCanopy.selectedNode.GetType(), defaultInspectorType);
                if (inspector != null && inspector.GetType() != inspectorType) inspector = null;
                inspector = CanopyInspectorBase.CreateEditor(targetScript, SelectedCanopy.selectedNode, inspectorType);
                inspector.SetProperties(targetScript, SelectedCanopy.selectedNode);
                CanopyInspectorBase.Draw(inspector);
            }


            if (GUI.changed)
            {
                EditorUtility.SetDirty(targetScript);
            }

            //DrawSeparator();
            //DrawDefaultInspector();
        }


        #endregion

        #region Helpers
        protected static List<Type> GetClassesWith(Type derivedType)
        {
            var assembly = Assembly.GetAssembly(derivedType);
            return assembly.GetTypes().Where(type => type.IsSubclassOf(derivedType)).ToList();
        }
        protected static Type FilterInspectorTypes(List<Type> types, Type renderType, Type defaultType)
        {
            Type finalType = defaultType;
            foreach (var type in types)
            {
                var canopyAttrib = (CanopyEditor)Attribute.GetCustomAttribute(type, typeof(CanopyEditor));
                if (canopyAttrib == null) continue;
                if (canopyAttrib.type == renderType)
                    finalType = type;
            }
            return finalType;
        }
        static public void DrawSeparator()
        {
            GUILayout.Space(12f);

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = blankTexture;
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.color = new Color(0f, 0f, 0f, 0.25f);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
                GUI.color = Color.white;
            }
        }
        static public Texture2D blankTexture
        {
            get
            {
                return EditorGUIUtility.whiteTexture;
            }
        }
        #endregion
    }
}
