﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    internal sealed class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public AsyncLock()
        {
        }

        public async Task<IDisposable> AcquireAsync()
        {
            await this.semaphore.WaitAsync();
            return new Releaser(this);
        }

        public void Release()
        {
            this.semaphore.Release();
        }

        public void Dispose()
        {
            this.semaphore.Dispose();
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock asyncLock;

            public Releaser(AsyncLock asyncLock)
            {
                this.asyncLock = asyncLock;
            }

            public void Dispose()
            {
                this.asyncLock.Release();
            }
        }
    }
}
