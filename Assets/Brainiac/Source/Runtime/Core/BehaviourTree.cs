using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
    /// <summary>
    /// 行为树，在实际运行中是反序列化出来的，而不是动态生成。
    /// </summary>
    public class BehaviourTree
    {
        [BTProperty("Root")] private Root m_root;

        [BTIgnore]
        public Root Root
        {
            get { return m_root; }
        }

        [BTIgnore] public bool ReadOnly { get; set; }

        public BehaviourTree()
        {
            if (m_root == null)
            {
                m_root = new Root();
                m_root.Position = new Vector2(0, 0);
            }
        }
    }
}