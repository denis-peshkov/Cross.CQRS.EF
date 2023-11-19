namespace Cross.CQRS.EF.Services;

internal interface IDbContextProvider
{
    DbContext Get();
}
