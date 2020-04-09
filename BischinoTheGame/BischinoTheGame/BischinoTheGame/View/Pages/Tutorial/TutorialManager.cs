using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BischinoTheGame.View.Pages.Tutorial
{
    public class TutorialManager
    {
        public static int _animationCount = 0;
        public static uint _lengthPerChar = 40;
        public static Task AutoWriteText(string text, Label label, IAnimatable owner)
        {
            var autoWriteTaskSource = new TaskCompletionSource<bool>();
            var length = text.Length;

            var writeAnimation = new Animation(
                val => Device.BeginInvokeOnMainThread(() => label.Text = text.Substring(0, (int) val)),
                0D, length);

            writeAnimation.Commit(owner, Convert.ToString(_animationCount++), 16U, (uint) (_lengthPerChar * length),
                Easing.Linear, (_, flag) => autoWriteTaskSource.SetResult(flag));

            return autoWriteTaskSource.Task;
        }
    }
}
