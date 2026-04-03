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

namespace Tayx.Graphy.Ram
{
    public class G_RamModule : G_Module
    {
        [SerializeField] private GameObject m_ramGraphGameObject;
        [SerializeField] private List<Image> m_backgroundImages = new();

        [Header("RAM")]
        [SerializeField] private Color m_allocatedRamColor = new Color32(255, 190, 60, 255);
        [SerializeField] private Color m_reservedRamColor = new Color32(205, 84, 229, 255);
        [SerializeField] private Color m_monoRamColor = new(0.3f, 0.65f, 1f, 1f);
        [Range(10, 300)] [SerializeField] private int m_ramGraphResolution = 150;
        [Range(1, 200)] [SerializeField] private int m_ramTextUpdateRate = 3;

        private readonly List<GameObject> m_childrenGameObjects = new();
        private G_RamGraph m_ramGraph;
        private G_RamMonitor m_ramMonitor;
        private G_RamText m_ramText;

        public GraphyMode CurrentGraphyMode => ManagerMode;

        public Color AllocatedRamColor => m_allocatedRamColor;

        public Color ReservedRamColor => m_reservedRamColor;

        public Color MonoRamColor => m_monoRamColor;

        public int RamGraphResolution => m_ramGraphResolution;

        public int RamTextUpdateRate => m_ramTextUpdateRate;

        private void Awake()
        {
            InitializeModule();

            m_ramGraph = GetComponent<G_RamGraph>();
            m_ramMonitor = GetComponent<G_RamMonitor>();
            m_ramText = GetComponent<G_RamText>();

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
                case ModuleState.Basic:
                    gameObject.SetActive(true);
                    m_childrenGameObjects.SetAllActive(true);
                    SetGraphActive(false);
                    SetBackgroundIndex(1);
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

            if (m_ramGraph != null)
            {
                m_ramGraph.UpdateParameters();
            }

            if (m_ramText != null)
            {
                m_ramText.UpdateParameters();
            }
        }

        public override void UpdateModule(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsVisibleState())
            {
                return;
            }

            if (m_ramMonitor != null)
            {
                m_ramMonitor.UpdateMonitor();
            }

            if (m_ramText != null)
            {
                m_ramText.UpdateText(unscaledDeltaTime);
            }

            if (CurrentState == ModuleState.Full && m_ramGraph != null)
            {
                m_ramGraph.UpdateGraphModule();
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
            if (m_ramGraph != null)
            {
                if (active)
                {
                    m_ramGraph.EnsureInitialized();
                }

                m_ramGraph.enabled = active;
            }

            if (m_ramGraphGameObject != null)
            {
                m_ramGraphGameObject.SetActive(active);
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
