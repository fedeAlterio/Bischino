using BischinoTheGame.Controller;
using BischinoTheGame.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BischinoTheGame.Helpers
{
    public class AnimationManager
    {
        private static long _totAnimations;

        public static Task<bool> AsyncAnimation(double start, double end, Action<double> callback, uint rate = 16, uint length = 250, Easing easing = null)
        {
            return Application.Current.MainPage.AsyncAnimation(GetAnimationName(), start, end, callback, rate, length, easing);
        }

        private static string GetAnimationName()
        {
            var ret = nameof(AnimationManager) + _totAnimations++;
            if (_totAnimations == long.MaxValue)
                _totAnimations = 0;
            return ret;
        }
    }
}
