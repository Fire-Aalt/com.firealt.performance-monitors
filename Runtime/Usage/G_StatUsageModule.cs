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

using System.Collections.Generic;
using Tayx.Graphy.UI;
using Tayx.Graphy.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Usage
{
    public class G_StatUsageModule : G_Module
    {
        [SerializeField] private GameObject m_usageGraphGameObject;
        [SerializeField] private List<GameObject> m_nonBasicTextGameObjects = new();
        [SerializeField] private List<Image> m_backgroundImages = new();

        [Header("Usage")]
        [SerializeField] private Color m_goodUsageColor = new Color32(118, 212, 58, 255);
        [SerializeField] private float m_goodUsageThresholdMs = 16.7f;
        [SerializeField] private Color m_cautionUsageColor = new Color32(243, 232, 0, 255);
        [SerializeField] private float m_cautionUsageThresholdMs = 33.3f;
        [SerializeField] private Color m_criticalUsageColor = new Color32(220, 41, 30, 255);
        [Range(10, 300)] [SerializeField] private int m_usageGraphResolution = 150;
        [Range(1, 200)] [SerializeField] private int m_usageTextUpdateRate = 3;

        private readonly List<GameObject> m_childrenGameObjects = new();
        private G_StatUsageGraph m_usageGraph;
        private G_StatUsageMonitor m_usageMonitor;
        private G_StatUsageText m_usageText;

        public GraphyMode CurrentGraphyMode => ManagerMode;

        public Color GoodUsageColor => m_goodUsageColor;

        public float GoodUsageThresholdMs => m_goodUsageThresholdMs;

        public Color CautionUsageColor => m_cautionUsageColor;

        public float CautionUsageThresholdMs => m_cautionUsageThresholdMs;

        public Color CriticalUsageColor => m_criticalUsageColor;

        public int UsageGraphResolution => m_usageGraphResolution;

        public int UsageTextUpdateRate => m_usageTextUpdateRate;

        private void Awake()
        {
            InitializeModule();

            m_usageGraph = GetComponent<G_StatUsageGraph>();
            m_usageMonitor = GetComponent<G_StatUsageMonitor>();
            m_usageText = GetComponent<G_StatUsageText>();

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

            if (m_usageGraph != null)
            {
                m_usageGraph.UpdateParameters();
            }

            if (m_usageMonitor != null)
            {
                m_usageMonitor.UpdateParameters();
            }

            if (m_usageText != null)
            {
                m_usageText.UpdateParameters();
            }
        }

        public override void UpdateModule(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsVisibleState())
            {
                return;
            }

            var showDetailedStats = CurrentState != ModuleState.Basic;

            if (m_usageMonitor != null)
            {
                m_usageMonitor.UpdateMonitor(showDetailedStats);
            }

            if (m_usageText != null)
            {
                m_usageText.UpdateText(unscaledDeltaTime, showDetailedStats);
            }

            if (CurrentState == ModuleState.Full && m_usageGraph != null)
            {
                m_usageGraph.UpdateGraphModule();
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
            if (m_usageGraph != null)
            {
                if (active)
                {
                    m_usageGraph.EnsureInitialized();
                }

                m_usageGraph.enabled = active;
            }

            if (m_usageGraphGameObject != null)
            {
                m_usageGraphGameObject.SetActive(active);
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
