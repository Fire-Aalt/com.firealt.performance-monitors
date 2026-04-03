/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            05-Dec-17
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

#if GRAPHY_XR
using UnityEngine.XR;
#endif

using System.Collections.Generic;
using System.Text;

using Tayx.Graphy.UI;
using Tayx.Graphy.Utils;
using Tayx.Graphy.Utils.NumString;
using TMPro;

namespace Tayx.Graphy.Advanced
{
    public class G_DeviceInfoModule : G_Module
    {
        #region Variables -> Serialized Private

        [Header("Device Info")]
        [SerializeField] private List<Image> m_backgroundImages = new();

        [SerializeField] private TextMeshProUGUI m_graphicsDeviceVersionText;

        [SerializeField] private TextMeshProUGUI m_processorTypeText;

        [SerializeField] private TextMeshProUGUI m_operatingSystemText;

        [SerializeField] private TextMeshProUGUI m_systemMemoryText;

        [SerializeField] private TextMeshProUGUI m_graphicsDeviceNameText;
        [SerializeField] private TextMeshProUGUI m_graphicsMemorySizeText;
        [SerializeField] private TextMeshProUGUI m_screenResolutionText;
        [SerializeField] private TextMeshProUGUI m_gameWindowResolutionText;
        [SerializeField] private TextMeshProUGUI m_gameVRResolutionText;
        
#if GRAPHY_XR
        private readonly List<XRDisplaySubsystem> m_displaySubsystems = new List<XRDisplaySubsystem>();
#endif
        
        [Range( 1, 60 )] [SerializeField] private float m_updateRate = 1f; // 1 update per sec.

        #endregion

        #region Variables -> Private

        private float m_deltaTime;

        private StringBuilder m_sb;

        private readonly string[] m_windowStrings =
        {
            "Window: ",
            "x",
            "@",
            "Hz",
            "[",
            "dpi]"
        };
        
        private readonly string[] m_vrStrings =
        {
            "VR: (",
            "*2)x",
            "@",
            "Hz"
        };

        #endregion

        #region Methods -> Unity Callbacks

        private void Awake()
        {
            if (m_modulePosition == ModulePosition.TopRight && m_moduleOffset == Vector2.zero)
            {
                m_modulePosition = ModulePosition.BottomLeft;
            }

            Init();
        }

        public override void UpdateModule(float deltaTime, float unscaledDeltaTime)
        {
            if (!IsVisibleState())
            {
                return;
            }

            m_deltaTime += unscaledDeltaTime;

            if( m_deltaTime > 1f / m_updateRate )
            {
                // Update screen window resolution
                m_sb.Length = 0;

                m_sb.Append( m_windowStrings[ 0 ] ).Append( Screen.width.ToStringNonAlloc() )
                    .Append( m_windowStrings[ 1 ] ).Append( Screen.height.ToStringNonAlloc() )
                    .Append( m_windowStrings[ 2 ] ).Append(
#if UNITY_2022_2_OR_NEWER
                        ((int)Screen.currentResolution.refreshRateRatio.value).ToStringNonAlloc()
#else
                        Screen.currentResolution.refreshRate.ToStringNonAlloc()
#endif
                        )
                    .Append( m_windowStrings[ 3 ] )
                    .Append( m_windowStrings[ 4 ] ).Append( ((int) Screen.dpi).ToStringNonAlloc() )
                    .Append( m_windowStrings[ 5 ] );

                m_gameWindowResolutionText.text = m_sb.ToString();

#if GRAPHY_XR
                // If XR enabled, update screen XR resolution
                if( XRSettings.enabled )
                {
                    m_sb.Length = 0;

#if UNITY_2020_2_OR_NEWER
                    SubsystemManager.GetSubsystems( m_displaySubsystems );
#else
                    SubsystemManager.GetInstances( m_displaySubsystems );
#endif
                    float refreshRate = -1;

                    if( m_displaySubsystems.Count > 0 )
                    {
                        m_displaySubsystems[ 0 ].TryGetDisplayRefreshRate( out refreshRate );
                    }

                    m_sb.Append( m_vrStrings[ 0 ] ).Append( XRSettings.eyeTextureWidth.ToStringNonAlloc() )
                        .Append( m_vrStrings[ 1 ] ).Append( XRSettings.eyeTextureHeight.ToStringNonAlloc() )
                        .Append( m_vrStrings[ 2 ] ).Append( Mathf.RoundToInt( refreshRate ).ToStringNonAlloc() )
                        .Append( m_vrStrings[ 3 ] );

                    m_gameVRResolutionText.text = m_sb.ToString();
                    m_gameVRResolutionText.gameObject.SetActive(true);
                }
#endif
                
                // Reset variables
                m_deltaTime = 0f;
            }
        }

        #endregion

        #region Methods -> Public

        public override void SetPosition(ModulePosition newModulePosition, Vector2 offset)
        {
            base.SetPosition(newModulePosition, offset);

            if (newModulePosition == ModulePosition.Free)
            {
                return;
            }

            switch( newModulePosition )
            {
                case ModulePosition.TopLeft:
                case ModulePosition.BottomLeft:

                    m_processorTypeText.alignment = TextAlignmentOptions.TopLeft;
                    m_systemMemoryText.alignment = TextAlignmentOptions.TopLeft;
                    m_graphicsDeviceNameText.alignment = TextAlignmentOptions.TopLeft;
                    m_graphicsDeviceVersionText.alignment = TextAlignmentOptions.TopLeft;
                    m_graphicsMemorySizeText.alignment = TextAlignmentOptions.TopLeft;
                    m_screenResolutionText.alignment = TextAlignmentOptions.TopLeft;
                    m_gameWindowResolutionText.alignment = TextAlignmentOptions.TopLeft;
                    m_gameVRResolutionText.alignment = TextAlignmentOptions.TopLeft;
                    m_operatingSystemText.alignment = TextAlignmentOptions.TopLeft;

                    break;

                case ModulePosition.TopRight:
                case ModulePosition.BottomRight:

                    m_processorTypeText.alignment = TextAlignmentOptions.TopRight;
                    m_systemMemoryText.alignment = TextAlignmentOptions.TopRight;
                    m_graphicsDeviceNameText.alignment = TextAlignmentOptions.TopRight;
                    m_graphicsDeviceVersionText.alignment = TextAlignmentOptions.TopRight;
                    m_graphicsMemorySizeText.alignment = TextAlignmentOptions.TopRight;
                    m_screenResolutionText.alignment = TextAlignmentOptions.TopRight;
                    m_gameWindowResolutionText.alignment = TextAlignmentOptions.TopRight;
                    m_gameVRResolutionText.alignment = TextAlignmentOptions.TopRight;
                    m_operatingSystemText.alignment = TextAlignmentOptions.TopRight;

                    break;
            }
        }

        public override void SetState(ModuleState state, bool silentUpdate = false)
        {
            base.SetState(state, silentUpdate);

            bool active = state == ModuleState.Full
                          || state == ModuleState.Text
                          || state == ModuleState.Basic;

            gameObject.SetActive( active );

            m_backgroundImages.SetAllActive( active && Background );
        }

        public override void UpdateParameters()
        {
            UpdateBackgroundImageColors(m_backgroundImages);
        }

        #endregion

        #region Methods -> Private

        private void Init()
        {
            G_IntString.Init( 0, 7680 );

            InitializeModule();

            m_sb = new StringBuilder();

            m_processorTypeText.text
                = "CPU: "
                  + SystemInfo.processorType
                  + " ["
                  + SystemInfo.processorCount
                  + " cores]";

            m_systemMemoryText.text
                = "RAM: "
                  + SystemInfo.systemMemorySize
                  + " MB";

            m_graphicsDeviceVersionText.text
                = "Graphics API: "
                  + SystemInfo.graphicsDeviceVersion;

            m_graphicsDeviceNameText.text
                = "GPU: "
                  + SystemInfo.graphicsDeviceName;

            m_graphicsMemorySizeText.text
                = "VRAM: "
                  + SystemInfo.graphicsMemorySize
                  + "MB. Max texture size: "
                  + SystemInfo.maxTextureSize
                  + "px. Shader level: "
                  + SystemInfo.graphicsShaderLevel;

            Resolution res = Screen.currentResolution;

            m_screenResolutionText.text
                = "Screen: "
                  + res.width
                  + "x"
                  + res.height
                  + "@"
#if UNITY_2022_2_OR_NEWER
                  + ((int)Screen.currentResolution.refreshRateRatio.value).ToStringNonAlloc()
#else
                  + res.refreshRate
#endif
                  + "Hz";

            m_operatingSystemText.text
                = "OS: "
                  + SystemInfo.operatingSystem
                  + " ["
                  + SystemInfo.deviceType
                  + "]";

            m_gameVRResolutionText.gameObject.SetActive(false);
            
            float preferredWidth = 0;

            // Resize the background overlay

            var texts = new List<TextMeshProUGUI>()
            {
                m_graphicsDeviceVersionText,
                m_processorTypeText,
                m_systemMemoryText,
                m_graphicsDeviceNameText,
                m_graphicsMemorySizeText,
                m_screenResolutionText,
                m_gameWindowResolutionText,
                m_gameVRResolutionText,
                m_operatingSystemText
            };

            foreach( var text in texts )
            {
                if( text.preferredWidth > preferredWidth )
                {
                    preferredWidth = text.preferredWidth;
                }
            }
            
            RectTransform.SetSizeWithCurrentAnchors
            (
                axis: RectTransform.Axis.Horizontal,
                size: preferredWidth + 25
            );

            RectTransform.anchoredPosition = new Vector2
            (
                x: RectTransform.anchoredPosition.x - RectTransform.rect.width / 2
                   + RectTransform.rect.width / 2 * Mathf.Sign( RectTransform.anchoredPosition.x ),
                y: RectTransform.anchoredPosition.y
            );

            SetOriginalPosition(RectTransform.anchoredPosition);

            UpdateParameters();
        }

        private bool IsVisibleState()
        {
            return CurrentState == ModuleState.Full
                   || CurrentState == ModuleState.Text
                   || CurrentState == ModuleState.Basic;
        }

        #endregion
    }
}
