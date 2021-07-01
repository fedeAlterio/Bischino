using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BischinoTheGame.Extensions
{

    public static class IAnimatableExtensions
    {
        public static Task<bool> AsyncAnimation(this IAnimatable @this, string name, double start, double end, Action<double> callback, uint rate = 16, uint length = 250,
            Easing easing = default)
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new Animation(callback, start, end);
            animation.Commit
            (
                owner: @this,
                name: name,
                rate: rate,
                length: length,
                easing: easing,
                (val, finished) =>
                {
                    callback.Invoke(end);
                    tcs.SetResult(finished);
                }
            );

            return tcs.Task;
        }
    }
}
