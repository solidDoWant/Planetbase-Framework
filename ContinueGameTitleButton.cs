using Planetbase;

namespace PlanetbaseFramework
{
    public class ContinueGameTitleButton : TitleButton
    {
        public ContinueGameTitleButton() : base("continue_game", SaveData.anySaveGames())
        {
        }

        public override void handleAction(GameStateTitle gst)
        {
            GameManager.getInstance().setGameStateGameContinue();
        }
    }
}
