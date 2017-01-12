using Planetbase;

namespace PlanetbaseFramework
{
    public class LoadGameTitleButton : TitleButton
    {
        public LoadGameTitleButton() : base("load_game", SaveData.anySaveGames())
        {
        }

        public override void handleAction(GameStateTitle gst)
        {
            GameManager.getInstance().setGameStateLoadGame();
        }
    }
}
