using Planetbase;

namespace PlanetbaseFramework
{
    public class ContinueGameTitleButton : TitleButton
    {
        public ContinueGameTitleButton() : base("continue_game", SaveData.anySaveGames(), true)
        {
        }

        public override void HandleAction(GameStateTitle gst)
        {
            GameManager.getInstance().setGameStateGameContinue();
        }
    }
}