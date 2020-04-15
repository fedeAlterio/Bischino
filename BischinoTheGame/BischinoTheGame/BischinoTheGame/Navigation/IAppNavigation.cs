using System.Threading.Tasks;
using BischinoTheGame.Navigation.RoomNavigation;
using BischinoTheGame.Navigation.TutorialNavigation;

namespace BischinoTheGame.Navigation
{
    public interface IAppNavigation
    {
        Task DisplayAlert(string title, string message);
        IGameNavigation GameNavigation { get; }
        ITutorialNavigation TutorialNavigation { get; }
    }
}
