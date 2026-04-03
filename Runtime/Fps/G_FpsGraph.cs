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

namespace Tayx.Graphy.Fps
{
    public class G_FpsGraph : G_Graph
    {
        [SerializeField] private Image m_imageGraph;
        [SerializeField] private Shader ShaderFull;
        [SerializeField] private Shader ShaderLight;

        private G_FpsModule _mFpsModule;
        private G_FpsMonitor m_fpsMonitor;
        private int m_resolution = 150;
        private G_GraphShader m_shaderGraph;
        private float[] m_graphSamples;
        private float m_highestFps;

        protected override void UpdateGraph()
        {
            if (m_graphSamples == null || m_shaderGraph.ShaderArrayValues == null)
            {
                CreatePoints();
            }

            m_fpsMonitor.PopulateGraphValues(m_graphSamples);

            var currentMaxFps = 0f;
            for (var i = 0; i < m_resolution; i++)
            {
                if (currentMaxFps < m_graphSamples[i])
                {
                    currentMaxFps = m_graphSamples[i];
                }
            }

            m_highestFps = m_highestFps < 1f || m_highestFps <= currentMaxFps ? currentMaxFps : m_highestFps - 1f;
            m_highestFps = m_highestFps > 0f ? m_highestFps : 1f;

            for (var i = 0; i < m_resolution; i++)
            {
                m_shaderGraph.ShaderArrayValues[i] = m_graphSamples[i] / m_highestFps;
            }

            m_shaderGraph.UpdatePoints();
            m_shaderGraph.Average = m_fpsMonitor.AverageFPS / m_highestFps;
            m_shaderGraph.UpdateAverage();
            m_shaderGraph.GoodThreshold = _mFpsModule.GoodFPSThreshold / m_highestFps;
            m_shaderGraph.CautionThreshold = _mFpsModule.CautionFPSThreshold / m_highestFps;
            m_shaderGraph.UpdateThresholds();
        }

        protected override void UpdateGraphParameters()
        {
            ConfigureShaderForMode(m_shaderGraph, _mFpsModule.CurrentGraphyMode, ShaderFull, ShaderLight);
            m_resolution = m_shaderGraph.GetArraySize(_mFpsModule.FpsGraphResolution);
            CreatePoints();
        }

        protected override void CreatePoints()
        {
            if (m_graphSamples == null || m_graphSamples.Length != m_resolution)
            {
                m_graphSamples = new float[m_resolution];
            }

            ResetShaderValues(m_shaderGraph, m_resolution);
            System.Array.Clear(m_graphSamples, 0, m_resolution);
            ApplyShaderStyle(m_shaderGraph, _mFpsModule.GoodFPSColor,
                _mFpsModule.CautionFPSColor, _mFpsModule.CriticalFPSColor);
        }

        protected override void InitializeGraph()
        {
            _mFpsModule = GetComponent<G_FpsModule>();
            m_fpsMonitor = GetComponent<G_FpsMonitor>();
            m_shaderGraph = new G_GraphShader
            {
                Image = m_imageGraph,
            };
        }
    }
}
