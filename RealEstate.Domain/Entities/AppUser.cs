using System;
namespace RealEstate.Domain.Entities
{
    public class AppUser
    {
        public Guid IdUser { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
