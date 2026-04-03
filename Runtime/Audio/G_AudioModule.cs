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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Audio
{
    public class G_AudioModule : G_Module
    {
        public enum LookForAudioListener
        {
            Always,
            OnSceneLoad,
            Never,
        }

        [Header("Audio")]
        [SerializeField] private GameObject m_audioGraphGameObject;
        [SerializeField] private TextMeshProUGUI m_audioDbText;
        [SerializeField] private List<Image> m_backgroundImages = new();

        [SerializeField] private LookForAudioListener m_findAudioListenerInCameraIfNull = LookForAudioListener.OnSceneLoad;
        [SerializeField] private AudioListener m_audioListener;
        [SerializeField] private Color m_audioGraphColor = Color.white;
        [Range(10, 300)] [SerializeField] private int m_audioGraphResolution = 81;
        [Range(1, 200)] [SerializeField] private int m_audioTextUpdateRate = 3;
        [SerializeField] private FFTWindow m_fftWindow = FFTWindow.Blackman;
        [Tooltip("Must be a power of 2 and between 64-8192")] [SerializeField] private int m_spectrumSize = 512;

        private readonly List<GameObject> m_childrenGameObjects = new();
        private G_AudioGraph m_audioGraph;
        private G_AudioMonitor m_audioMonitor;
        private G_AudioText m_audioText;

        public GraphyMode CurrentGraphyMode => ManagerMode;

        public LookForAudioListener AudioListenerSearchMode => m_findAudioListenerInCameraIfNull;

        public AudioListener AudioListener => m_audioListener;

        public Color AudioGraphColor => m_audioGraphColor;

        public int AudioGraphResolution => m_audioGraphResolution;

        public int AudioTextUpdateRate => m_audioTextUpdateRate;

        public FFTWindow FftWindow => m_fftWindow;

        public int SpectrumSize => m_spectrumSize;

        private void Awake()
        {
            InitializeModule();

            m_audioGraph = GetComponent<G_AudioGraph>();
            m_audioMonitor = GetComponent<G_AudioMonitor>();
            m_audioText = GetComponent<G_AudioText>();

            foreach (Transform child in transform)
            {
                if (child.parent == transform)
                {
                    m_childrenGameObjects.Add(child.gameObject);
                }
            }
        }

        public override void SetPosition(ModulePosition newModulePosition, Vector2 offset)
        {
            base.SetPosition(newModulePosition, offset);

            if (m_audioDbText == null)
            {
                return;
            }

            switch (newModulePosition)
            {
                case ModulePosition.TopLeft:
                case ModulePosition.BottomLeft:
                    m_audioDbText.alignment = TextAlignmentOptions.TopLeft;
                    break;

                case ModulePosition.TopRight:
                case ModulePosition.BottomRight:
                    m_audioDbText.alignment = TextAlignmentOptions.TopRight;
                    break;
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

            if (m_audioGraph != null)
            {
                m_audioGraph.UpdateParameters();
            }

            if (m_audioMonitor != null)
            {
                m_audioMonitor.UpdateParameters();
            }

            if (m_audioText != null)
            {
                m_audioText.UpdateParameters();
            }
        }

        public override void UpdateModule(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsVisibleState())
            {
                return;
            }

            var updateSpectrumData = CurrentState == ModuleState.Full;

            if (m_audioMonitor != null)
            {
                m_audioMonitor.UpdateMonitor(deltaTime, updateSpectrumData);
            }

            if (m_audioText != null)
            {
                m_audioText.UpdateText(deltaTime);
            }

            if (updateSpectrumData && m_audioGraph != null)
            {
                m_audioGraph.UpdateGraphModule();
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
            if (m_audioGraph != null)
            {
                if (active)
                {
                    m_audioGraph.EnsureInitialized();
                }

                m_audioGraph.enabled = active;
            }

            if (m_audioGraphGameObject != null)
            {
                m_audioGraphGameObject.SetActive(active);
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
