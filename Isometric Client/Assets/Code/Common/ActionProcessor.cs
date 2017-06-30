using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Common
{
    public class ActionProcessor : SingletonBehaviour<ActionProcessor>
    {
        private readonly Queue<ActionPair> _syncActionQueue = new Queue<ActionPair>();
        private readonly object _actionQueueLock = new object();



        public void AddActionToQueue(Action action, TimeSpan delay = new TimeSpan())
        {
            AddActionToQueue(
                () =>
                {
                    action();
                    return true;
                }, 
                delay);
        }

        public void AddActionToQueue(Func<bool> action, TimeSpan delay = new TimeSpan())
        {
            lock (_actionQueueLock)
            {
                _syncActionQueue.Enqueue(new ActionPair
                {
                    Action = action,
                    DelayMax = delay,
                    CurrentDelay = delay,
                });
            }
        }

        private void FixedUpdate()
        {
            lock (_actionQueueLock)
            {
                if (_syncActionQueue.Count > 0)
                {
                    var syncActionPair = _syncActionQueue.Peek();

                    syncActionPair.CurrentDelay -= TimeSpan.FromSeconds(Time.fixedDeltaTime);

                    if (syncActionPair.CurrentDelay < TimeSpan.Zero)
                    {
                        syncActionPair.CurrentDelay = syncActionPair.DelayMax;

                        if (syncActionPair.Action())
                        {
                            _syncActionQueue.Dequeue();
                        }
                    }
                }
            }
        }

        private class ActionPair
        {
            public TimeSpan DelayMax, CurrentDelay;

            public Func<bool> Action;
        }
    }
}