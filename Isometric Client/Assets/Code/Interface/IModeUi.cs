using System;
using Isometric.Core.Vectors;

namespace Assets.Code.Interface
{
    public interface IModeUi
    {
        /// <summary>
        /// requests and shows actual data
        /// </summary>
        void Refresh();

        /// <summary>
        /// shows again last received data
        /// </summary>
        void ShowPreviousData();

        /// <summary>
        /// shows full information about cell
        /// </summary>
        /// <param name="position">position of cell</param>
        /// <returns>should cell be highlighted as selected</returns>
        bool SelectCell(Vector position);

        /// <summary>
        /// shows short information about cell
        /// </summary>
        /// <param name="position">position of cell</param>
        void HighlightCell(Vector position);

        /// <summary>
        /// is called every frame
        /// </summary>
        /// <param name="deltaTime"></param>
        void Update(TimeSpan deltaTime);

        /// <summary>
        /// clears Ui
        /// </summary>
        void Clear();
    }
}