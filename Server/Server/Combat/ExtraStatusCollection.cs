/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Players;
using Server.Maps;
using Server.Moves;
using System.Collections.Concurrent;
using System.Threading;

namespace Server.Combat
{
    public class ExtraStatusCollection
    {

        private List<ExtraStatus> extraStatus;
        ReaderWriterLockSlim rwLock;

        public ExtraStatusCollection() {
            extraStatus = new List<ExtraStatus>();
            rwLock = new ReaderWriterLockSlim();
        }

        public void Add(ExtraStatus status) {
            rwLock.EnterWriteLock();
            try {
                extraStatus.Add(status);
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        public void Clear() {
            rwLock.EnterWriteLock();
            try {
                extraStatus.Clear();
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        public void Remove(ExtraStatus status) {
            rwLock.EnterWriteLock();
            try {
                extraStatus.Remove(status);
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        public ExtraStatus GetStatus(string statusName) {
            rwLock.EnterReadLock();
            try {
                foreach (ExtraStatus vs in extraStatus) {
                    if (vs.Name == statusName) {
                        return vs;
                    }
                }
                return null;
            } finally {
                rwLock.ExitReadLock();
            }
        }

        public List<ExtraStatus> GetStatuses(params string[] statusNames) {
            rwLock.EnterReadLock();
            try {
                List<ExtraStatus> statuses = new List<ExtraStatus>((statusNames.Length > extraStatus.Count) ? statusNames.Length : extraStatus.Count);
                foreach (ExtraStatus vs in extraStatus) {
                    for (int i = 0; i < statusNames.Length; i++) {
                        if (vs.Name == statusNames[i]) {
                            statuses.Add(vs);
                            break;
                        }
                    }
                }

                return statuses;
            } finally {
                rwLock.ExitReadLock();
            }
        }

        public int Count {
            get {
                rwLock.EnterReadLock();
                try {
                    return extraStatus.Count;
                } finally {
                    rwLock.ExitReadLock();
                }
            }
        }

        public ExtraStatus this[int index] {
            get {
                rwLock.EnterReadLock();
                try {
                    return extraStatus[index];
                } finally {
                    rwLock.ExitReadLock();
                }
            }
            set {
                rwLock.EnterWriteLock();
                try {
                    extraStatus[index] = value;
                } finally {
                    rwLock.ExitWriteLock();
                }
            }
        }


    }
}
