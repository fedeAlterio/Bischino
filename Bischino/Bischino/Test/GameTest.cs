using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bischino.Test
{
    public class GameTest
    {
        public async void Test1()
        {
        }

        private async void AsyncCode(TaskCompletionSource<bool> completionSource)
        {
           
        }

        private void Ensure(bool predicate)
        {
            if (!predicate)
                throw new Exception("Ensure failed");
        }
    }
}
