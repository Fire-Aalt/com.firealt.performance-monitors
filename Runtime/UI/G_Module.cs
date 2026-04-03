/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            02-Apr-26
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Tayx.Graphy.UI
{
    public abstract class G_Module : MonoBehaviour, IGraphyModule, IGraphyRuntimeModule
    {
        [Header("Module")]
        [SerializeField] protected string m_moduleId = string.Empty;
        [SerializeField] protected ModulePosition m_modulePosition = ModulePosition.TopRight;
        [SerializeField] protected Vector2 m_moduleOffset = Vector2.zero;
        [SerializeField] protected ModuleState m_moduleState = ModuleState.Full;

        private GraphyManager m_graphyManager;
        private ModuleState m_previousModuleState = ModuleState.Full;
        private ModuleState m_currentModuleState = ModuleState.Full;
        private RectTransform m_rectTransform;
        private Vector2 m_originalPosition = Vector2.zero;

        public string ModuleId => m_moduleId;

        public ModulePosition ConfiguredPosition => m_modulePosition;

        public Vector2 ConfiguredOffset => m_moduleOffset;

        public ModuleState ConfiguredState => m_moduleState;

        protected GraphyManager GraphyManager => m_graphyManager;

        protected GraphyMode ManagerMode => m_graphyManager != null ? m_graphyManager.GraphyMode : GraphyMode.Full;

        protected bool Background => m_graphyManager != null && m_graphyManager.Background;

        protected Color BackgroundColor => m_graphyManager != null ? m_graphyManager.BackgroundColor : Color.clear;

        protected ModuleState CurrentState => m_currentModuleState;

        protected RectTransform RectTransform => m_rectTransform;

        protected Vector2 OriginalPosition => m_originalPosition;

        protected void InitializeModule()
        {
            m_graphyManager = transform.GetComponentInParent<GraphyManager>(true);
            m_previousModuleState = m_moduleState;
            m_currentModuleState = m_moduleState;

            m_rectTransform = GetComponent<RectTransform>();
            if (m_rectTransform != null)
            {
                m_originalPosition = m_rectTransform.anchoredPosition;
            }
        }

        public void ApplyConfiguration()
        {
            SetPosition(m_modulePosition, m_moduleOffset);

            m_previousModuleState = m_moduleState;
            SetState(m_moduleState, true);
        }

        public virtual void RefreshParameters()
        {
            UpdateParameters();
            SetPosition(m_modulePosition, m_moduleOffset);
            SetState(m_currentModuleState, true);
        }

        public virtual void RestorePreviousState()
        {
            SetState(m_previousModuleState);
        }

        public virtual void SetPosition(ModulePosition newModulePosition, Vector2 offset)
        {
            m_modulePosition = newModulePosition;
            m_moduleOffset = offset;
            ApplyPositionToRectTransform(newModulePosition, offset);
        }

        public virtual void SetState(ModuleState state, bool silentUpdate = false)
        {
            if (!silentUpdate)
            {
                m_previousModuleState = m_currentModuleState;
            }

            m_currentModuleState = state;
        }

        public abstract void UpdateParameters();

        public virtual void UpdateModule(float deltaTime, float unscaledDeltaTime)
        {
        }

        private void ApplyPositionToRectTransform(ModulePosition newModulePosition, Vector2 offset)
        {
            if (m_rectTransform == null || newModulePosition == ModulePosition.Free)
            {
                return;
            }

            m_rectTransform.anchoredPosition = m_originalPosition;

            var xSideOffset = Mathf.Abs(m_rectTransform.anchoredPosition.x) + offset.x;
            var ySideOffset = Mathf.Abs(m_rectTransform.anchoredPosition.y) + offset.y;

            switch (newModulePosition)
            {
                case ModulePosition.TopLeft:
                    m_rectTransform.anchorMax = Vector2.up;
                    m_rectTransform.anchorMin = Vector2.up;
                    m_rectTransform.pivot = Vector2.up;
                    m_rectTransform.anchoredPosition = new Vector2(xSideOffset, -ySideOffset);
                    break;

                case ModulePosition.TopRight:
                    m_rectTransform.anchorMax = Vector2.one;
                    m_rectTransform.anchorMin = Vector2.one;
                    m_rectTransform.pivot = Vector2.one;
                    m_rectTransform.anchoredPosition = new Vector2(-xSideOffset, -ySideOffset);
                    break;

                case ModulePosition.BottomLeft:
                    m_rectTransform.anchorMax = Vector2.zero;
                    m_rectTransform.anchorMin = Vector2.zero;
                    m_rectTransform.pivot = Vector2.zero;
                    m_rectTransform.anchoredPosition = new Vector2(xSideOffset, ySideOffset);
                    break;

                case ModulePosition.BottomRight:
                    m_rectTransform.anchorMax = Vector2.right;
                    m_rectTransform.anchorMin = Vector2.right;
                    m_rectTransform.pivot = Vector2.right;
                    m_rectTransform.anchoredPosition = new Vector2(-xSideOffset, ySideOffset);
                    break;
            }
        }

        protected void SetOriginalPosition(Vector2 originalPosition)
        {
            m_originalPosition = originalPosition;
        }

        protected void UpdateBackgroundImageColors(IEnumerable<Image> backgroundImages)
        {
            if (backgroundImages == null)
            {
                return;
            }

            foreach (var image in backgroundImages)
            {
                if (image != null)
                {
                    image.color = BackgroundColor;
                }
            }
        }
    }
}
