using Planetbase;

namespace PlanetbaseFramework
{
    public class LoadGameTitleButton : TitleButton
    {
        public LoadGameTitleButton() : base("load_game", SaveData.anySaveGames(), true)
        {
        }

        public override void HandleAction(GameStateTitle gst)
        {
            GameManager.getInstance().setGameStateLoadGame();
        }
    }
}