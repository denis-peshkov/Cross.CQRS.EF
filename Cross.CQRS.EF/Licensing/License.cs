namespace Cross.CQRS.Licensing;

internal class License
{
    internal License(params Claim[] claims)
        : this(new ClaimsPrincipal(new ClaimsIdentity(claims)))
    {

    }

    public License(ClaimsPrincipal claims)
    {
        if (Guid.TryParse(claims.FindFirst("sub_id")?.Value, out var subscriptionId))
        {
            SubscriptionId = subscriptionId;
        }

        if (Guid.TryParse(claims.FindFirst("user_id")?.Value, out var userId))
        {
            UserId = userId;
        }

        if (long.TryParse(claims.FindFirst("iat")?.Value, out var iat))
        {
            var startedAt = DateTimeOffset.FromUnixTimeSeconds(iat);
            StartDate = startedAt;
        }

        if (long.TryParse(claims.FindFirst("nbf")?.Value, out var nbf))
        {
            var notBefore = DateTimeOffset.FromUnixTimeSeconds(nbf);
            NotBeforeDate = notBefore;
        }

        if (long.TryParse(claims.FindFirst("exp")?.Value, out var exp))
        {
            var expiredAt = DateTimeOffset.FromUnixTimeSeconds(exp);
            ExpirationDate = expiredAt;
        }

        if (Enum.TryParse<EditionEnum>(claims.FindFirst("edition")?.Value, out var edition))
        {
            Edition = edition;
        }

        if (Enum.TryParse<ProductTypeEnum>(claims.FindFirst("type")?.Value, out var productType))
        {
            ProductType = productType;
        }

        IsConfigured = SubscriptionId != null
                       && UserId != null
                       && NotBeforeDate != null
                       && StartDate != null
                       && ExpirationDate != null
                       && Edition != null
                       && ProductType != null;
    }

    public Guid? UserId { get; }
    public Guid? SubscriptionId { get; }
    public DateTimeOffset? StartDate { get; }
    public DateTimeOffset? NotBeforeDate { get; }
    public DateTimeOffset? ExpirationDate { get; }
    public EditionEnum? Edition { get; }
    public ProductTypeEnum? ProductType { get; }

    public bool IsConfigured { get; }
}
