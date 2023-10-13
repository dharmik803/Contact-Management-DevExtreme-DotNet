using System;
using System.Collections.Generic;

namespace Contact_management_devextreme.Models;

public partial class RefreshTokens
{
    public string Username { get; set; } = null!;

    public string? TokenId { get; set; }

    public string? RefreshToken { get; set; }

    public bool? IsActive { get; set; }
}
