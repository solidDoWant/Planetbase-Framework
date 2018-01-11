using Planetbase;

namespace PlanetbaseFramework
{
    class FrameworkMod : ModBase
    {
        public FrameworkMod()
        {
            Utils.ErrorTexture = ModTextures.Find(x => x.name.Equals("error.png"));
        }

        public override string ModName => "Planetbase Framework";

        public override void Init()
        {
            ModTitleButton modTitleButton = new ModTitleButton();
        }

        private class ModTitleButton : TitleButton
        {
            public ModTitleButton() : base("mod_titlemenu", true)
            {
            }

            public override void HandleAction(GameStateTitle gst)
            {
                GameManager.getInstance().setNewState(new ModListGameState());
            }
        }
    }
}