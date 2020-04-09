using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BischinoTheGame.View
{
    public interface IAnimationPack
    {
        Task FadeText(Action<string> updatesCallback, string text);
        Task AutoWriteText(Action<string> updatesCallback, string text, int lengthPerChar = 40, Easing easing = null);
    }
}
