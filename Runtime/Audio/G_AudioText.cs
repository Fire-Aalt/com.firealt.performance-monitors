/* ---------------------------------------
 * Author:          Martin Pane (martintayx@gmail.com) (@martinTayx)
 * Contributors:    https://github.com/Tayx94/graphy/graphs/contributors
 * Project:         Graphy - Ultimate Stats Monitor
 * Date:            15-Dec-17
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

namespace Tayx.Graphy.Audio
{
    public class G_AudioText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_DBText;

        private G_AudioModule _mAudioModule;
        private G_AudioMonitor m_audioMonitor;
        private int m_updateRate = 4;
        private float m_deltaTimeOffset;

        private void Awake()
        {
            G_IntString.Init(-80, 0);

            _mAudioModule = GetComponent<G_AudioModule>();
            m_audioMonitor = GetComponent<G_AudioMonitor>();

            UpdateParameters();
        }

        public void UpdateText(float deltaTime)
        {
            if (!m_audioMonitor.SpectrumDataAvailable)
            {
                return;
            }

            if (m_deltaTimeOffset > 1f / m_updateRate)
            {
                m_deltaTimeOffset = 0f;
                m_DBText.text = Mathf.Clamp((int)m_audioMonitor.MaxDB, -80, 0).ToStringNonAlloc();
            }
            else
            {
                m_deltaTimeOffset += deltaTime;
            }
        }

        public void UpdateParameters()
        {
            m_updateRate = _mAudioModule.AudioTextUpdateRate;
        }
    }
}
