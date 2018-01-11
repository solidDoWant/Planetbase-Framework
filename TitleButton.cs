using System.Collections.Generic;

namespace PlanetbaseFramework
{
    public abstract class TitleButton
    {
        public string Name { get; }
        public bool EnableGui { get; }
        public bool IsBaseGameTitleButton { get; }

        private static readonly LinkedList<TitleButton> AllTitleButtons = new LinkedList<TitleButton>();

        static TitleButton()
        {
            new NewGameTitleButton();
            new ContinueGameTitleButton();
            new LoadGameTitleButton();
            new TutorialTitleButton();
            new ChallengesTitleButton();
            new SettingsTitleButton();
            new QuitTitleButton();
        }

        protected internal TitleButton(string nameKey, bool enableGui, bool isBaseGameTitleButton)
        {
            Name = Planetbase.StringList.get(nameKey);
            EnableGui = enableGui;
            IsBaseGameTitleButton = isBaseGameTitleButton;

            AllTitleButtons.AddLast(this);
        }

        protected TitleButton(string nameKey, bool enableGui) : this(nameKey, enableGui, false)
        {
            
        }

        public static LinkedList<TitleButton> GetAllTitleButtons()
        {
            return AllTitleButtons;
        }

        public abstract void HandleAction(Planetbase.GameStateTitle gst);
    }
}