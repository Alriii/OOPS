namespace MiniEcoMarket
{
    public abstract class User
    {
        public string Name { get; private set; }

        protected User(string name)
        {
            Name = name;
        }

        public abstract void DisplayInfo();
    }
}