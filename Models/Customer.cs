namespace BookStore.Models
{
    public partial class Customer
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string Address { get; set; } = null!;
    }
}
