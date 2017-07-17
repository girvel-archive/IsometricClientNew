using System;

namespace Assets.Code.Building
{
    public interface IIndicatorManager
    {
        void Update(Indicator indicator, TimeSpan deltaTime);

        void End(Indicator indicator);
    }
}