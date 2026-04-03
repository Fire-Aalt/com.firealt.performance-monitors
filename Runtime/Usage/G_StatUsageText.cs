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

using Tayx.Graphy.Utils.NumString;
using TMPro;
using UnityEngine;

namespace Tayx.Graphy.Usage
{
    public class G_StatUsageText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_msText;
        [SerializeField] private TextMeshProUGUI m_avgMsText;
        [SerializeField] private TextMeshProUGUI m_onePercentMsText;
        [SerializeField] private TextMeshProUGUI m_zero1PercentMsText;

        private G_StatUsageModule m_statUsageModule;
        private G_StatUsageMonitor m_statUsageMonitor;
        private int m_updateRate = 4;
        private int m_frameCount;
        private float m_deltaTime;
        private bool _firstFrame = true;

        private void Awake()
        {
            G_FloatString.Init(0, 1000);

            m_statUsageModule = GetComponent<G_StatUsageModule>();
            m_statUsageMonitor = GetComponent<G_StatUsageMonitor>();

            UpdateParameters();
        }

        public void UpdateText(float unscaledDeltaTime, bool showDetailedStats)
        {
            if (m_statUsageMonitor == null)
            {
                return;
            }

            m_deltaTime += unscaledDeltaTime;
            m_frameCount++;

            if (m_deltaTime <= 1f / m_updateRate && !_firstFrame)
            {
                return;
            }
            _firstFrame = false;

            UpdateUsageText(m_msText, m_statUsageMonitor.CurrentUsage);

            if (showDetailedStats)
            {
                UpdateUsageText(m_avgMsText, m_statUsageMonitor.AverageUsage);
                UpdateUsageText(m_onePercentMsText, m_statUsageMonitor.OnePercentHighestUsage);
                UpdateUsageText(m_zero1PercentMsText, m_statUsageMonitor.Zero1PercentHighestUsage);
            }

            m_deltaTime = 0f;
            m_frameCount = 0;
        }

        public void UpdateParameters()
        {
            m_updateRate = m_statUsageModule != null
                ? m_statUsageModule.UsageTextUpdateRate
                : 4;
        }

        private void UpdateUsageText(TextMeshProUGUI text, float usage)
        {
            if (text == null)
            {
                return;
            }

            text.text = usage.ToStringNonAlloc("0.0");
            SetUsageRelatedTextColor(text, usage);
        }

        private void SetUsageRelatedTextColor(TextMeshProUGUI text, float usage)
        {
            if (m_statUsageModule == null)
            {
                return;
            }

            if (usage <= m_statUsageModule.GoodUsageThresholdMs)
            {
                text.color = m_statUsageModule.GoodUsageColor;
            }
            else if (usage <= m_statUsageModule.CautionUsageThresholdMs)
            {
                text.color = m_statUsageModule.CautionUsageColor;
            }
            else
            {
                text.color = m_statUsageModule.CriticalUsageColor;
            }
        }
    }
}
