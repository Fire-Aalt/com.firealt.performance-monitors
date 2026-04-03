/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            22-Nov-17
 * Studio:          Tayx
 *
 * Git repo:        https://github.com/Tayx94/graphy
 *
 * This project is released under the MIT license.
 * Attribution is not required, but it is always welcomed!
 * -------------------------------------*/

using Tayx.Graphy.Utils.NumString;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Fps
{
    public class G_FpsText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_fpsText;
        [SerializeField] private TextMeshProUGUI m_msText;
        [SerializeField] private TextMeshProUGUI m_avgFpsText;
        [SerializeField] private TextMeshProUGUI m_onePercentFpsText;
        [SerializeField] private TextMeshProUGUI m_zero1PercentFpsText;

        private const string MsStringFormat = "0.0";

        private G_FpsModule _mFpsModule;
        private G_FpsMonitor m_fpsMonitor;
        private int m_updateRate = 4;
        private int m_frameCount;
        private float m_deltaTime;
        private float m_fps;
        private float m_ms;

        private void Awake()
        {
            G_IntString.Init(0, 2000);
            G_FloatString.Init(0, 100);

            _mFpsModule = GetComponent<G_FpsModule>();
            m_fpsMonitor = GetComponent<G_FpsMonitor>();

            UpdateParameters();
        }

        public void UpdateText(float unscaledDeltaTime, bool showDetailedStats)
        {
            m_deltaTime += unscaledDeltaTime;
            m_frameCount++;

            if (m_deltaTime > 1f / m_updateRate || m_fps == 0)
            {
                m_fps = m_frameCount / m_deltaTime;

                m_fpsText.text = Mathf.RoundToInt(m_fps).ToStringNonAlloc();
                SetFpsRelatedTextColor(m_fpsText, m_fps);

                if (showDetailedStats)
                {
                    m_ms = m_deltaTime / m_frameCount * 1000f;
                    m_msText.text = m_ms.ToStringNonAlloc(MsStringFormat);
                    SetFpsRelatedTextColor(m_msText, m_fps);

                    m_onePercentFpsText.text = ((int)m_fpsMonitor.OnePercentFPS).ToStringNonAlloc();
                    SetFpsRelatedTextColor(m_onePercentFpsText, m_fpsMonitor.OnePercentFPS);

                    m_zero1PercentFpsText.text = ((int)m_fpsMonitor.Zero1PercentFps).ToStringNonAlloc();
                    SetFpsRelatedTextColor(m_zero1PercentFpsText, m_fpsMonitor.Zero1PercentFps);

                    m_avgFpsText.text = ((int)m_fpsMonitor.AverageFPS).ToStringNonAlloc();
                    SetFpsRelatedTextColor(m_avgFpsText, m_fpsMonitor.AverageFPS);
                }

                m_deltaTime = 0f;
                m_frameCount = 0;
            }
        }

        public void UpdateParameters()
        {
            m_updateRate = _mFpsModule.FpsTextUpdateRate;
        }

        private void SetFpsRelatedTextColor(TextMeshProUGUI text, float fps)
        {
            var roundedFps = Mathf.RoundToInt(fps);

            if (roundedFps >= _mFpsModule.GoodFPSThreshold)
            {
                text.color = _mFpsModule.GoodFPSColor;
            }
            else if (roundedFps >= _mFpsModule.CautionFPSThreshold)
            {
                text.color = _mFpsModule.CautionFPSColor;
            }
            else
            {
                text.color = _mFpsModule.CriticalFPSColor;
            }
        }
    }
}
