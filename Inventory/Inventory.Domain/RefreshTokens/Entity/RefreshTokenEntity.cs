namespace Inventory.Domain.RefreshTokens.Entity
{
    /// <summary>
    /// Un refresh token emitido a un usuario. El valor crudo nunca se guarda
    /// aquí ni en ningún lado: solo su hash (TokenHash). Vive el tiempo que
    /// dure su ventana (ExpiresAtUtc) o hasta que se revoque explícitamente
    /// (logout, rotación al refrescar).
    /// </summary>
    public class RefreshTokenEntity
    {
        private RefreshTokenEntity() { }

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string TokenHash { get; private set; } = default!;
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public DateTime? RevokedAtUtc { get; private set; }

        public static RefreshTokenEntity Create(int userId, string tokenHash, DateTime nowUtc, DateTime expiresAtUtc)
        {
            return new RefreshTokenEntity
            {
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAtUtc = nowUtc,
                ExpiresAtUtc = expiresAtUtc
            };
        }

        /// <summary>Utilizable: ni vencido ni revocado.</summary>
        public bool IsActive(DateTime nowUtc) => RevokedAtUtc is null && ExpiresAtUtc > nowUtc;

        public void Revoke(DateTime nowUtc) => RevokedAtUtc ??= nowUtc;
    }
}
