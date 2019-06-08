namespace PlanetbaseFramework
{
    public abstract class TitleButton
    {
        public string Name { get; }

        protected TitleButton(string nameKey)
        {
            Name = Planetbase.StringList.get(nameKey);
        }
        
        public abstract void HandleAction(Planetbase.GameStateTitle gst);
    }
}