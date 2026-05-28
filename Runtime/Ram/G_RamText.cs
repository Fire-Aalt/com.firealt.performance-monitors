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

using Tayx.Graphy.Utils.NumString;
using TMPro;
using UnityEngine;

namespace Tayx.Graphy.Ram
{
    public class G_RamText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_allocatedSystemMemorySizeText;
        [SerializeField] private TextMeshProUGUI m_reservedSystemMemorySizeText;
        [SerializeField] private TextMeshProUGUI m_monoSystemMemorySizeText;

        private G_RamModule _mRamModule;
        private G_RamMonitor m_ramMonitor;
        private float m_updateRate = 4f;
        private float m_deltaTime;

        private void Awake()
        {
            G_IntString.Init(0, 16386);

            _mRamModule = GetComponent<G_RamModule>();
            m_ramMonitor = GetComponent<G_RamMonitor>();

            UpdateParameters();
        }

        public void UpdateText(float unscaledDeltaTime)
        {
            m_deltaTime += unscaledDeltaTime;

            if (m_deltaTime > 1f / m_updateRate)
            {
                m_allocatedSystemMemorySizeText.text = ((int)m_ramMonitor.AllocatedRam).ToStringNonAlloc();
                m_reservedSystemMemorySizeText.text = ((int)m_ramMonitor.ReservedRam).ToStringNonAlloc();
                m_monoSystemMemorySizeText.text = ((int)m_ramMonitor.MonoRam).ToStringNonAlloc();
                m_deltaTime = 0f;
            }
        }

        public void UpdateParameters()
        {
            m_allocatedSystemMemorySizeText.color = _mRamModule.AllocatedRamColor;
            m_reservedSystemMemorySizeText.color = _mRamModule.ReservedRamColor;
            m_monoSystemMemorySizeText.color = _mRamModule.MonoRamColor;
            m_updateRate = _mRamModule.RamTextUpdateRate;
        }
    }
}
