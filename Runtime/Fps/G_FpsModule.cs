/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            03-Jan-18
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using System.Collections.Generic;
using Tayx.Graphy.UI;
using Tayx.Graphy.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Fps
{
    public class G_FpsModule : G_Module
    {
        [SerializeField] private GameObject m_fpsGraphGameObject;
        [SerializeField] private List<GameObject> m_nonBasicTextGameObjects = new();
        [SerializeField] private List<Image> m_backgroundImages = new();

        [Header("FPS")]
        [SerializeField] private Color m_goodFpsColor = new Color32(118, 212, 58, 255);
        [SerializeField] private int m_goodFpsThreshold = 60;
        [SerializeField] private Color m_cautionFpsColor = new Color32(243, 232, 0, 255);
        [SerializeField] private int m_cautionFpsThreshold = 30;
        [SerializeField] private Color m_criticalFpsColor = new Color32(220, 41, 30, 255);
        [Range(10, 300)] [SerializeField] private int m_fpsGraphResolution = 150;
        [Range(1, 200)] [SerializeField] private int m_fpsTextUpdateRate = 3;

        private readonly List<GameObject> m_childrenGameObjects = new();
        private G_FpsGraph m_fpsGraph;
        private G_FpsMonitor m_fpsMonitor;
        private G_FpsText m_fpsText;

        public GraphyMode CurrentGraphyMode => ManagerMode;

        public Color GoodFPSColor => m_goodFpsColor;

        public int GoodFPSThreshold => m_goodFpsThreshold;

        public Color CautionFPSColor => m_cautionFpsColor;

        public int CautionFPSThreshold => m_cautionFpsThreshold;

        public Color CriticalFPSColor => m_criticalFpsColor;

        public int FpsGraphResolution => m_fpsGraphResolution;

        public int FpsTextUpdateRate => m_fpsTextUpdateRate;

        private void Awake()
        {
            InitializeModule();

            m_fpsGraph = GetComponent<G_FpsGraph>();
            m_fpsMonitor = GetComponent<G_FpsMonitor>();
            m_fpsText = GetComponent<G_FpsText>();

            foreach (Transform child in transform)
            {
                if (child.parent == transform)
                {
                    m_childrenGameObjects.Add(child.gameObject);
                }
            }
        }

        public override void SetState(ModuleState state, bool silentUpdate = false)
        {
            base.SetState(state, silentUpdate);

            switch (state)
            {
                case ModuleState.Full:
                    gameObject.SetActive(true);
                    m_childrenGameObjects.SetAllActive(true);
                    SetGraphActive(true);
                    SetBackgroundIndex(0);
                    break;

                case ModuleState.Text:
                    gameObject.SetActive(true);
                    m_childrenGameObjects.SetAllActive(true);
                    SetGraphActive(false);
                    SetBackgroundIndex(1);
                    break;

                case ModuleState.Basic:
                    gameObject.SetActive(true);
                    m_childrenGameObjects.SetAllActive(true);
                    m_nonBasicTextGameObjects.SetAllActive(false);
                    SetGraphActive(false);
                    SetBackgroundIndex(2);
                    break;

                case ModuleState.Background:
                    gameObject.SetActive(true);
                    m_childrenGameObjects.SetAllActive(false);
                    SetGraphActive(false);
                    DisableBackgrounds();
                    break;

                case ModuleState.Off:
                    gameObject.SetActive(false);
                    break;
            }
        }

        public override void UpdateParameters()
        {
            UpdateBackgroundImageColors(m_backgroundImages);

            if (m_fpsGraph != null)
            {
                m_fpsGraph.UpdateParameters();
            }

            if (m_fpsMonitor != null)
            {
                m_fpsMonitor.UpdateParameters();
            }

            if (m_fpsText != null)
            {
                m_fpsText.UpdateParameters();
            }
        }

        public override void UpdateModule(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsVisibleState())
            {
                return;
            }

            var showDetailedStats = CurrentState != ModuleState.Basic;

            if (m_fpsMonitor != null)
            {
                m_fpsMonitor.UpdateMonitor(unscaledDeltaTime, showDetailedStats);
            }

            if (m_fpsText != null)
            {
                m_fpsText.UpdateText(unscaledDeltaTime, showDetailedStats);
            }

            if (CurrentState == ModuleState.Full && m_fpsGraph != null)
            {
                m_fpsGraph.UpdateGraphModule();
            }
        }

        private void DisableBackgrounds()
        {
            if (m_backgroundImages != null)
            {
                m_backgroundImages.SetAllActive(false);
            }
        }

        private void SetBackgroundIndex(int index)
        {
            if (m_backgroundImages == null)
            {
                return;
            }

            if (Background)
            {
                m_backgroundImages.SetOneActive(index);
            }
            else
            {
                m_backgroundImages.SetAllActive(false);
            }
        }

        private void SetGraphActive(bool active)
        {
            if (m_fpsGraph != null)
            {
                if (active)
                {
                    m_fpsGraph.EnsureInitialized();
                }

                m_fpsGraph.enabled = active;
            }

            if (m_fpsGraphGameObject != null)
            {
                m_fpsGraphGameObject.SetActive(active);
            }
        }

        private bool IsVisibleState()
        {
            return CurrentState == ModuleState.Full
                   || CurrentState == ModuleState.Text
                   || CurrentState == ModuleState.Basic;
        }
    }
}
