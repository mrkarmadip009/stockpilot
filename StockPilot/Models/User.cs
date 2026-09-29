using Microsoft.AspNetCore.Identity;

namespace StockPilot.Models
{
public class User : IdentityUser
{
public string Name { get; set; } = "";
}
}
