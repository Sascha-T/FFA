namespace FFA.Database.Models
{
    public class User
    {
        public User() { }

        public User(ulong id)
        {
            Id = id;
        }

        public ulong Id { get; set; }
        public int Reputation { get; set; } = 0;
    }
}
