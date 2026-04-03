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

using Tayx.Graphy.Graph;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Audio
{
    public class G_AudioGraph : G_Graph
    {
        [SerializeField] private Image m_imageGraph;
        [SerializeField] private Image m_imageGraphHighestValues;
        [SerializeField] private Shader ShaderFull;
        [SerializeField] private Shader ShaderLight;

        private G_AudioModule _mAudioModule;
        private G_AudioMonitor m_audioMonitor;
        private int m_resolution = 40;
        private G_GraphShader m_shaderGraph;
        private G_GraphShader m_shaderGraphHighestValues;
        private float[] m_graphArray;
        private float[] m_graphArrayHighestValue;

        protected override void UpdateGraph()
        {
            if (!m_audioMonitor.SpectrumDataAvailable)
            {
                return;
            }

            var incrementPerIteration = Mathf.Max(1, Mathf.FloorToInt(m_audioMonitor.Spectrum.Length / (float)m_resolution));

            for (var i = 0; i < m_resolution; i++)
            {
                var currentValue = 0f;
                for (var j = 0; j < incrementPerIteration; j++)
                {
                    var sampleIndex = Mathf.Min(i * incrementPerIteration + j, m_audioMonitor.Spectrum.Length - 1);
                    currentValue += m_audioMonitor.Spectrum[sampleIndex];
                }

                if ((i + 1) % 3 == 0 && i > 1)
                {
                    var value =
                        (m_audioMonitor.dBNormalized(m_audioMonitor.lin2dB(currentValue / incrementPerIteration)) +
                         m_graphArray[i - 1] +
                         m_graphArray[i - 2]) / 3f;

                    m_graphArray[i] = value;
                    m_graphArray[i - 1] = value;
                    m_graphArray[i - 2] = -1f;
                }
                else
                {
                    m_graphArray[i] =
                        m_audioMonitor.dBNormalized(m_audioMonitor.lin2dB(currentValue / incrementPerIteration));
                }
            }

            for (var i = 0; i < m_resolution; i++)
            {
                m_shaderGraph.ShaderArrayValues[i] = m_graphArray[i];
            }

            m_shaderGraph.UpdatePoints();

            for (var i = 0; i < m_resolution; i++)
            {
                var currentValue = 0f;
                for (var j = 0; j < incrementPerIteration; j++)
                {
                    var sampleIndex = Mathf.Min(
                        i * incrementPerIteration + j,
                        m_audioMonitor.SpectrumHighestValues.Length - 1);
                    currentValue += m_audioMonitor.SpectrumHighestValues[sampleIndex];
                }

                if ((i + 1) % 3 == 0 && i > 1)
                {
                    var value =
                        (m_audioMonitor.dBNormalized(m_audioMonitor.lin2dB(currentValue / incrementPerIteration)) +
                         m_graphArrayHighestValue[i - 1] +
                         m_graphArrayHighestValue[i - 2]) / 3f;

                    m_graphArrayHighestValue[i] = value;
                    m_graphArrayHighestValue[i - 1] = value;
                    m_graphArrayHighestValue[i - 2] = -1f;
                }
                else
                {
                    m_graphArrayHighestValue[i] =
                        m_audioMonitor.dBNormalized(m_audioMonitor.lin2dB(currentValue / incrementPerIteration));
                }
            }

            for (var i = 0; i < m_resolution; i++)
            {
                m_shaderGraphHighestValues.ShaderArrayValues[i] = m_graphArrayHighestValue[i];
            }

            m_shaderGraphHighestValues.UpdatePoints();
        }

        protected override void UpdateGraphParameters()
        {
            ConfigureShaderForMode(m_shaderGraph, _mAudioModule.CurrentGraphyMode, ShaderFull, ShaderLight);
            ConfigureShaderForMode(m_shaderGraphHighestValues, _mAudioModule.CurrentGraphyMode, ShaderFull, ShaderLight);
            m_resolution = m_shaderGraph.GetArraySize(_mAudioModule.AudioGraphResolution);
            CreatePoints();
        }

        protected override void CreatePoints()
        {
            if (m_graphArray == null || m_graphArray.Length != m_resolution)
            {
                m_graphArray = new float[m_resolution];
                m_graphArrayHighestValue = new float[m_resolution];
            }

            ResetShaderValues(m_shaderGraph, m_resolution);
            ResetShaderValues(m_shaderGraphHighestValues, m_resolution);
            System.Array.Clear(m_graphArray, 0, m_resolution);
            System.Array.Clear(m_graphArrayHighestValue, 0, m_resolution);

            ApplyShaderStyle(m_shaderGraph, _mAudioModule.AudioGraphColor,
                _mAudioModule.AudioGraphColor, _mAudioModule.AudioGraphColor);
            ApplyShaderStyle(m_shaderGraphHighestValues, _mAudioModule.AudioGraphColor,
                _mAudioModule.AudioGraphColor, _mAudioModule.AudioGraphColor);
        }

        protected override void InitializeGraph()
        {
            _mAudioModule = GetComponent<G_AudioModule>();
            m_audioMonitor = GetComponent<G_AudioMonitor>();

            m_shaderGraph = new G_GraphShader { Image = m_imageGraph };
            m_shaderGraphHighestValues = new G_GraphShader { Image = m_imageGraphHighestValues };
        }
    }
}
