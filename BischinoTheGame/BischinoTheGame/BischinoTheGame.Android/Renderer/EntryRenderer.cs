using Android.Content;
using BischinoTheGame.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using EntryRenderer = BischinoTheGame.Droid.Renderer.EntryRenderer;

[assembly: ExportRenderer(typeof(Entry), typeof(EntryRenderer))]
namespace BischinoTheGame.Droid.Renderer
{
    public class EntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
    {
        public EntryRenderer(Context context) : base(context)
        {
            
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}