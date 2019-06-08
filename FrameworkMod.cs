using System;
using System.Reflection;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class FrameworkMod : ModBase
    {
        public new static Version ModVersion => Assembly.GetExecutingAssembly().GetName().Version;

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

        public static void Callback(string condition, string stackTrace, LogType type)
        {

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