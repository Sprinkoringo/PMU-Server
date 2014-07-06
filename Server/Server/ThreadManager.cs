using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
    public class ThreadManager
    {
        public static void SetMaxThreads(int workerThreads, int completionPortThreads) {
            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
        }

        public static void StartOnThreadParams(WaitCallback waitCallback, object param) {
            ThreadPool.QueueUserWorkItem(waitCallback, param);
        }

        public static void StartOnThread(WaitCallback waitCallback) {
            ThreadPool.QueueUserWorkItem(waitCallback);
        }
    }
}
