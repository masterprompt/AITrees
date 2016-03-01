using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Tangatek.AITrees;

namespace Tangatek.AITreesEditor
{
    public abstract class TreeEditorBase : EditorWindow
    {
        #region Properties
        protected static EditorWindow _window;

        protected static AITree selectedTree = null;

        private static bool reload = false;
        private bool contextRequired = false;
        private Vector2 workspaceScroll = Vector2.zero;
        #endregion

        #region TreeEditorBase
        protected virtual List<Type> GetAvailableBranches()
        {
            return GetClassesWith(typeof(Branch));
        }
        protected virtual List<Type> GetAvailableLeaves()
        {
            var leaves = GetClassesWith(typeof(AITrees.Action));
            leaves.AddRange(GetClassesWith(typeof(Condition)));
            return leaves;
        }
        protected virtual List<Type> GetAvailableTwigs()
        {
            return GetClassesWith(typeof(Decorator));
        }
        #endregion


        #region EditorWindow
        void OnSelectionChange()
        {
            TreeInspectorBase.ClearCanopy();
            //	Replaint regardless of selection changes
            Repaint();
        }
        void Update()
        {
            //  Get window size
            CanopyRenderer.lineSize.x = position.width;

            //	Do we need to serialize from some change in the inspector?
            //CheckSerialization();
        }


        void OnGUI()
        {

            //	Render a scroll
            workspaceScroll = GUI.BeginScrollView(
                new Rect(0, 0, position.width, position.height),
                workspaceScroll,
                new Rect(0, 0, 1000, 1000)
            );

            //	Render the map
            CanopyRenderer.Render(TreeInspectorBase.SelectedCanopy);
            //	Do context if needed
            ContextMenu();

            GUI.EndScrollView();
            HandleEvents();
        }


        #endregion

        #region Events
        private void HandleEvents()
        {
            if (Application.isPlaying) return;
            var evt = Event.current;
            switch (evt.type)
            {
                case EventType.MouseDown:
                    TreeInspectorBase.SelectedCanopy.selectedNode = CanopyRenderer.Select(TreeInspectorBase.SelectedCanopy);
                    //map.Select(evt.mousePosition);
                    Repaint();
                    break;
                case EventType.ContextClick:
                    TreeInspectorBase.SelectedCanopy.selectedNode = CanopyRenderer.Select(TreeInspectorBase.SelectedCanopy);
                    //map.Select(evt.mousePosition);
                    Repaint();
                    contextRequired = true;
                    evt.Use();
                    break;

                case EventType.KeyUp:
                    switch (evt.keyCode)
                    {
                        case KeyCode.Delete:
                        case KeyCode.Backspace:
                            if (focusedWindow != this) break;
                            //ContextDeleteNode(map.selectedNode);
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region Helpers
        protected static List<Type> GetClassesWith(Type derivedType)
        {
            var assembly = Assembly.GetAssembly(derivedType);
            return assembly.GetTypes().Where(type => type.IsSubclassOf(derivedType)).ToList();
        }
        #endregion

        #region ContextMenu
        private void ContextMenu()
        {
            if (Application.isPlaying) return;
            if (!contextRequired) return;
            contextRequired = false;
            //  Create context menu
            var menu = new GenericMenu();

            Context_ShowMove(ref menu);
            Context_ShowBranches(ref menu);
            Context_ShowLimbs(ref menu);
            Context_ShowLeaves(ref menu);

            Context_ShowDelete(ref menu);
            //  Show context menu
            menu.ShowAsContext();
        }

        #region Menu Items
        private void Context_ShowMove(ref GenericMenu menu)
        {
            if (TreeInspectorBase.SelectedCanopy == null) return;
            /*
            if (map.selectedCanopyObject == null) return;
            if (map.selectedPoint == null) return;
            if (map.selectedPoint.parentBranch == null) return;

            menu.AddItem(new GUIContent("Move/Up"), false, this.MoveObjectUp);
            menu.AddItem(new GUIContent("Move/Down"), false, this.MoveObjectDown);
             * */
        }
        private void Context_ShowBranches(ref GenericMenu menu)
        {
            if (TreeInspectorBase.SelectedCanopy == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedNode == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedBranch == null && TreeInspectorBase.SelectedCanopy.selectedLimb == null) return;
            foreach (var type in GetAvailableBranches())
                menu.AddItem(new GUIContent("Add/" + ContextPathForType(type)), false, this.AddToBranch, type.AssemblyQualifiedName);
        }
        private void Context_ShowLimbs(ref GenericMenu menu)
        {
            if (TreeInspectorBase.SelectedCanopy == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedNode == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedBranch == null && TreeInspectorBase.SelectedCanopy.selectedLimb == null) return;
            foreach (var type in GetAvailableTwigs())
                menu.AddItem(new GUIContent("Add/" + ContextPathForType(type)), false, this.AddToBranch, type.AssemblyQualifiedName);
        }
        private void Context_ShowLeaves(ref GenericMenu menu)
        {
            if (TreeInspectorBase.SelectedCanopy == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedNode == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedBranch == null && TreeInspectorBase.SelectedCanopy.selectedLimb == null) return;
            foreach (var type in GetAvailableLeaves())
                menu.AddItem(new GUIContent("Add/" + ContextPathForType(type)), false, this.AddToBranch, type.AssemblyQualifiedName);
        }

        private void Context_ShowDelete(ref GenericMenu menu)
        {
            if (TreeInspectorBase.SelectedCanopy == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedNode == null) return;
            string name = TreeInspectorBase.SelectedCanopy.selectedNode.name;
            menu.AddItem(new GUIContent("Delete" + name), false, this.ContextDeleteNode, TreeInspectorBase.SelectedCanopy.selectedNode);
        }
        private static string ContextPathForType(Type type)
        {
            var pathAttrib = (AddCanopyMenu)Attribute.GetCustomAttribute(type, typeof(AddCanopyMenu));
            if (pathAttrib == null) return type.Name;
            var path = pathAttrib.canopyMenu;
            if (path[path.Length - 1].ToString() == "/") path = path + type.Name;
            return path;
        }
        #endregion

        #region Delegates
        public void AddToBranch(object dataObject)
        {
            if (TreeInspectorBase.SelectedCanopy == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedNode == null) return;
            if (TreeInspectorBase.SelectedCanopy.selectedBranch == null && TreeInspectorBase.SelectedCanopy.selectedLimb == null) return;

            var type = Type.GetType((string)dataObject);
            Debug.Log("Adding Type:" + type.Name);
            if (type == null) return;

            //  Create the node
            var node = TreeInspectorBase.SelectedCanopy.CreateNode(type);
            if (node == null) return;
            Debug.Log("Adding Node:" + node.name);

            if (TreeInspectorBase.SelectedCanopy.selectedBranch != null) TreeInspectorBase.SelectedCanopy.selectedBranch.AddChild(node);
            if (TreeInspectorBase.SelectedCanopy.selectedLimb != null) TreeInspectorBase.SelectedCanopy.selectedLimb.child = node;

            //  Serialize the canopy
            Debug.Log("Serializing");

            Debug.Log("Canopy:" + TreeInspectorBase.SelectedCanopy);
            Debug.Log("Tree:" + TreeInspectorBase.SelectedCanopy.tree);
            Debug.Log("SerializedCanopy:" + TreeInspectorBase.SelectedCanopy.tree.serializedCanopy);
            TreeInspectorBase.SelectedCanopy.tree.serializedCanopy = SerializedCanopy.Create(TreeInspectorBase.SelectedCanopy);

        }
        public void ContextDeleteNode(object dataObject)
        {
            /*
            if (dataObject == null) return;
            if (dataObject is CanopyObject)
                foreach (var trunk in selectedTree.trunks)
                    trunk.DeleteObjectInChildren((CanopyObject)dataObject);

            if (dataObject is Trunk)
                selectedTree.trunks.Remove((Trunk)dataObject);

            selectedTree.Serialize();
            reload = true;
             * */
        }

        #endregion
        #endregion
    }
}
