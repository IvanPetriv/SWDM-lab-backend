namespace Domain.Entities;
public class RefreshToken {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string HashedToken { get; set; } = string.Empty;
    public bool IsValid { get; set; } = false;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    #region Navigations
    public User User { get; set; } = null!;
    #endregion
}
