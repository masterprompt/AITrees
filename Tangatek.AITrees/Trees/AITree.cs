using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Tangatek.AITrees
{
    [AddComponentMenu("AITrees/Tree")]
    public class AITree : MonoBehaviour, NamedObject
    {
        #region Serialization
        [SerializeField]
        //[HideInInspector]
        public SerializedCanopy serializedCanopy = new SerializedCanopy();
        #endregion

        #region Canopy
        private Canopy _canopy;
        public Canopy canopy { get { return _canopy; } set { _canopy = value; } }
        #endregion

        #region Monobehaviour
        public virtual void Awake()
        {
            canopy = Canopy.Create(serializedCanopy, this);
            if (canopy != null) canopy.OnAwake();
        }
        public virtual void Update()
        {
            if (canopy != null) canopy.OnUpdate();
        }
        public virtual void OnDestroy()
        {
            if (canopy != null) canopy.OnDestroy();
        }
        #endregion
    }
}
