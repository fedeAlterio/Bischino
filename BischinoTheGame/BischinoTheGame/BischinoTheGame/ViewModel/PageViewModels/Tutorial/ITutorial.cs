using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BischinoTheGame.ViewModel.PageViewModels.Tutorial
{
    public interface ITutorial
    {
        Task AutoWriteText(string text); 
        Task WriteText(string text);
        Task WelcomeCompleted();
        Task FadeOrderedCards();
        Task FadeSeeds();
        Task ShowFirstPaolo();
        Task DuplicatePaolo();
        Task MaximumPaolo();
        Task MinimumPaolo();
        Task PaoloCompleted();
        Task FadeLives();
        Task DropCards();
        Task YourTurn();
        Task DropTheCard();
        Task AddPoint();
        Task Restart();
        Task StrongCardsPopup();
        Task PaoloPopup();
        Task ShowPrediction();
        Task ShowOtherPlayersPredictions();
        Task FadeOtherPlayersPrediction();
        Task PredictionWinComparison();
        Task LoseALife();
        Task AnimateRound2();
        Task LastRound();
    }
}
