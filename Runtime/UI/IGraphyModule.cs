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

using UnityEngine;

namespace Tayx.Graphy.UI
{
    public interface IGraphyModule : IMovable, IModifiableState
    {
        string ModuleId { get; }

        ModulePosition ConfiguredPosition { get; }

        Vector2 ConfiguredOffset { get; }

        ModuleState ConfiguredState { get; }

        void ApplyConfiguration();

        void UpdateParameters();

        void RefreshParameters();

        void RestorePreviousState();
    }
}
