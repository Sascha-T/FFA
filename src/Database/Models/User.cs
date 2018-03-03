namespace FFA.Database.Models
{
    public sealed class User : Entity
    {
        public ulong UserId { get; set; }
        public double Reputation { get; set; }
    }
}
