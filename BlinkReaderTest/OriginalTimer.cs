using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BlinkReaderTest
{
    internal class OriginalTimer
    {
        public static async IAsyncEnumerable<long> PeridoricTimerAsync([EnumeratorCancellation] CancellationToken token, double fps = 60.0)
        {
            Winmm.timeBeginPeriod(1);
            long TPF = (long)(10_000_000 / fps);//Ticks Per Frame
            var nextFrame = DateTime.Now.Ticks;

            while (!token.IsCancellationRequested)
            {
                await Task.Delay((int)(1000 / fps));
                var currentTick = DateTime.Now.Ticks;
                while (currentTick < nextFrame) currentTick = DateTime.Now.Ticks;
                nextFrame += TPF;
                yield return currentTick;
            }
        }
    }

    internal class Winmm
    {
        [DllImport("Winmm.dll")]
        internal static extern uint timeBeginPeriod(uint uuPeriod);
    }
}
