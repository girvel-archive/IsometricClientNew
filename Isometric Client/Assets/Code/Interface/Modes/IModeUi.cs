using System;
using Isometric.Core.Vectors;

namespace Assets.Code.Interface.Modes
{
    public interface IModeUi
    {
        void Refresh();

        void SelectCell(Vector position);

        void HighlightCell(Vector position);

        void Update(TimeSpan deltaTime);

        void ShowPreviousData();

        void Clear();
    }
}