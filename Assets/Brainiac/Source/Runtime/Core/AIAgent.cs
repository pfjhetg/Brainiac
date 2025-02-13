using UnityEngine;
using UnityEngine.Events;

namespace Brainiac
{
    [RequireComponent(typeof(Blackboard))]
    public class AIAgent : MonoBehaviour
    {
        public event UnityAction BeforeUpdate;
        public event UnityAction AfterUpdate;

        /// <summary>
        /// 行为树源数据（配置文件）
        /// </summary>
        [SerializeField] private BTAsset m_behaviourTree;

        [SerializeField] private GameObject m_body;
        [SerializeField] private UpdateMode m_updateMode;
        [SerializeField] private float m_updateInterval;
        /// <summary>
        /// 调试模式，可以断点暂停
        /// </summary>
        [SerializeField] private bool m_debugMode;

        /// <summary>
        /// 行为树类
        /// </summary>
        private BehaviourTree m_btInstance;

        private Blackboard m_blackboard;
        private float m_timeElapsedSinceLastUpdate;
        private bool m_isRunning;

        public GameObject Body
        {
            get { return m_body != null ? m_body : gameObject; }
        }

        public Blackboard Blackboard
        {
            get { return m_blackboard; }
        }

        public bool DebugMode
        {
            get { return m_debugMode; }
            set { m_debugMode = value; }
        }

        private void Awake()
        {
            m_blackboard = gameObject.GetComponent<Blackboard>();

            if (m_behaviourTree != null)
            {
                // 根据配置文件初始化出行为树
                m_btInstance = m_behaviourTree.CreateRuntimeTree();
            }

            m_timeElapsedSinceLastUpdate = 0.0f;
            m_isRunning = true;
        }

        private void Start()
        {
            if (m_btInstance != null)
            {
                m_btInstance.Root.OnStart(this);
            }
        }

        private void Update()
        {
            if (m_updateMode != UpdateMode.Manual && m_isRunning)
            {
                if (m_updateMode == UpdateMode.EveryFrame | m_timeElapsedSinceLastUpdate >= m_updateInterval)
                {
                    UpdateInternal();
                    m_timeElapsedSinceLastUpdate = 0.0f;
                }

                m_timeElapsedSinceLastUpdate += Time.deltaTime;
            }
        }

        /// <summary>
        /// 逻辑循环入口
        /// </summary>
        private void UpdateInternal()
        {
            if (m_btInstance != null)
            {
                RaiseBeforeUpdateEvent();

                if (m_btInstance.Root.Status != BehaviourNodeStatus.Running)
                {
                    m_btInstance.Root.OnReset();
                }

                // 从树的根部开始按照逻辑调用子树的行为
                m_btInstance.Root.Run(this);

                RaiseAfterUpdateEvent();
            }
        }

        public void Stop()
        {
            if (m_updateMode != UpdateMode.Manual)
            {
                m_timeElapsedSinceLastUpdate = 0.0f;
                m_isRunning = false;
                if (m_btInstance != null)
                {
                    m_btInstance.Root.OnReset();
                }
            }
#if UNITY_EDITOR
            else
            {
                if (m_btInstance != null)
                {
                    m_btInstance.Root.OnReset();
                }
            }
#endif
        }

        public void Pause()
        {
            if (m_updateMode != UpdateMode.Manual)
            {
                m_isRunning = false;
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning("Can't pause AIAgent! Update mode is set to 'Manual'.", this);
            }
#endif
        }

        public void Resume()
        {
            if (m_updateMode != UpdateMode.Manual)
            {
                m_isRunning = true;
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning("Can't resume AIAgent! Update mode is set to 'Manual'.", this);
            }
#endif
        }

        public void Tick()
        {
            if (m_updateMode == UpdateMode.Manual)
            {
                UpdateInternal();
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning("Can't tick AIAgent! Update mode needs to be set to 'Manual'.", this);
            }
#endif
        }

#if UNITY_EDITOR
        public BehaviourTree GetBehaviourTree()
        {
            return m_btInstance;
        }
#endif
        private void RaiseBeforeUpdateEvent()
        {
            if (BeforeUpdate != null)
            {
                BeforeUpdate();
            }
        }

        private void RaiseAfterUpdateEvent()
        {
            if (AfterUpdate != null)
            {
                AfterUpdate();
            }
        }
    }
}