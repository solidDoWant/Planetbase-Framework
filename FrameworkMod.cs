using System;
using System.Collections.Generic;
using Planetbase;

namespace PlanetbaseFramework
{
    public class FrameworkMod : ModBase
    {
        public const string AssemblyVersion = "2.3.3.0";
        public new static readonly Version ModVersion = new Version(AssemblyVersion);

        public FrameworkMod()
        {
            Utils.ErrorTexture = ModTextures.Find(x => x.name.Equals("error.png"));
        }

        public override string ModName { get; } = "Planetbase Framework";

        public override void Init()
        {
            RegisterTitleButton(new ModTitleButton());

            InjectPatches();
        }

        public override ICollection<string> GetContributors()
        {
            return new[] { "solidDoWant" };
        }

        private class ModTitleButton : TitleButton
        {
            public ModTitleButton() : base("mod_titlemenu") { }

            public override void HandleAction(GameStateTitle gst)
            {
                GameManager.getInstance().setNewState(new ModListGameState());
            }
        }
    }
}