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

using Tayx.Graphy.Graph;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Usage
{
    public class G_StatUsageGraph : G_Graph
    {
        private const float MinimumGraphHeight = 1f;
        private const float AdditionalHeadroomMs = 1f;
        private const float AdditionalHeadroomScale = 0.1f;
        private const float GrowthSnapThresholdMs = 0.5f;
        private const float GrowthLerpFactor = 0.25f;
        private const float ShrinkLerpFactor = 0.08f;

        [SerializeField] private Image m_imageGraph;
        [SerializeField] private Shader ShaderFull;
        [SerializeField] private Shader ShaderLight;

        private G_StatUsageModule m_statUsageModule;
        private G_StatUsageMonitor m_statUsageMonitor;
        private int m_resolution = 150;
        private G_GraphShader m_shaderGraph;
        private float[] m_graphSamples;
        private float m_highestUsage;

        protected override void UpdateGraph()
        {
            if (m_graphSamples == null || m_shaderGraph.ShaderArrayValues == null)
            {
                CreatePoints();
            }

            m_statUsageMonitor.PopulateGraphValues(m_graphSamples);

            var currentMaxUsage = 0f;
            for (var i = 0; i < m_resolution; i++)
            {
                if (currentMaxUsage < m_graphSamples[i])
                {
                    currentMaxUsage = m_graphSamples[i];
                }
            }

            var targetHighestUsage = Mathf.Max(MinimumGraphHeight,
                Mathf.Max(currentMaxUsage + AdditionalHeadroomMs, currentMaxUsage * (1f + AdditionalHeadroomScale)));

            if (m_highestUsage < MinimumGraphHeight)
            {
                m_highestUsage = targetHighestUsage;
            }
            else if (targetHighestUsage > m_highestUsage)
            {
                var growth = targetHighestUsage - m_highestUsage;
                m_highestUsage = growth >= GrowthSnapThresholdMs
                    ? targetHighestUsage
                    : Mathf.Lerp(m_highestUsage, targetHighestUsage, GrowthLerpFactor);
            }
            else
            {
                m_highestUsage = Mathf.Lerp(m_highestUsage, targetHighestUsage, ShrinkLerpFactor);
            }

            for (var i = 0; i < m_resolution; i++)
            {
                m_shaderGraph.ShaderArrayValues[i] = m_graphSamples[i] / m_highestUsage;
            }

            m_shaderGraph.UpdatePoints();
            m_shaderGraph.Average = m_statUsageMonitor.AverageUsage / m_highestUsage;
            m_shaderGraph.UpdateAverage();
            m_shaderGraph.GoodThreshold = m_statUsageModule.CautionUsageThresholdMs / m_highestUsage;
            m_shaderGraph.CautionThreshold = m_statUsageModule.GoodUsageThresholdMs / m_highestUsage;
            m_shaderGraph.UpdateThresholds();
        }

        protected override void UpdateGraphParameters()
        {
            if (m_statUsageModule == null || m_shaderGraph == null)
            {
                return;
            }

            ConfigureShaderForMode(m_shaderGraph, m_statUsageModule.CurrentGraphyMode, ShaderFull, ShaderLight);
            m_resolution = m_shaderGraph.GetArraySize(m_statUsageModule.UsageGraphResolution);
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
            ApplyShaderStyle(m_shaderGraph, m_statUsageModule.CriticalUsageColor,
                m_statUsageModule.CautionUsageColor, m_statUsageModule.GoodUsageColor);
        }

        protected override void InitializeGraph()
        {
            m_statUsageModule = GetComponent<G_StatUsageModule>();
            m_statUsageMonitor = GetComponent<G_StatUsageMonitor>();
            m_shaderGraph = new G_GraphShader
            {
                Image = m_imageGraph,
            };
        }
    }
}
