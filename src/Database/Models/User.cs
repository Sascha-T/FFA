namespace FFA.Database.Models
{
    public sealed class User : Entity
    {
        public ulong UserId { get; set; }
        public int Reputation { get; set; } = 0;
    }
}
