using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BischinoTheGame.View.Extensions
{
    public static class AnimatableExtensions
    {
        private static long _animationCount = 0;
        public static Task AutomateText(this IAnimatable owner, string text, Action<string> onNewText, int lengthPerChar = 40, Easing easing = null)
        {
            var autoWriteTaskSource = new TaskCompletionSource<bool>();
            var length = text.Length;

            var writeAnimation = new Animation(
                val =>  onNewText.Invoke(text.Substring(0, (int)val)),
                0D, length);

            writeAnimation.Commit(owner, Convert.ToString(_animationCount++), 16U, (uint)(lengthPerChar * length),
               easing, (_, flag) => autoWriteTaskSource.SetResult(flag));

            return autoWriteTaskSource.Task;
        }
    }
}
