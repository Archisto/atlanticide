namespace Atlanticide.Persistence
{
    public class SaveSystem
    {
        IPersistence persistence;

        public SaveSystem(IPersistence persistence)
        {
            this.persistence = persistence;
        }

        public void Save(GameData data)
        {
            persistence.Save(data);
        }

        public GameData Load()
        {
            return persistence.Load<GameData>();
        }
    }
}
