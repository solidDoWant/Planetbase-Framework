using System;
using System.Reflection;
using Planetbase;

namespace PlanetbaseFramework
{
    public class FrameworkMod : ModBase
    {
        public override Version ModVersion => Assembly.GetExecutingAssembly().GetName().Version;

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