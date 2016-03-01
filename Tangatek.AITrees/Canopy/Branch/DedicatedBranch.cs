using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangatek.AITrees
{
    public class DedicatedBranch : Branch
    {
        public int defaultIndex = 0;
        private int currentIndex = 0;
        public override void OnAwake()
        {
            base.OnAwake();
            currentIndex = defaultIndex;
        }
        public override State OnUpdate()
        {
            if (children.Count == 0) return State.Failure;
            if (currentIndex < 0) return State.Failure;
            if (currentIndex >= children.Count) return State.Failure;
            return children[currentIndex].OnUpdate();
        }
    }
}
