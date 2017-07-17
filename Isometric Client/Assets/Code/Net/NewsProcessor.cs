using System;
using System.Collections.Generic;
using Isometric.Dtos;

namespace Assets.Code.Net
{
    public class NewsProcessor
    {
        private Dictionary<string, Action<NewsDto>> _behaviour;



        public NewsProcessor(Dictionary<string, Action<NewsDto>> behaviour)
        {
            _behaviour = behaviour;
        }

        public void Process(NewsDto news)
        {
            _behaviour[news.Type](news);
        }
    }
}