namespace Application.Abstractions.Messaging;

public interface IPagedQuery<TResponse> : IQuery<TResponse>
{
    int Page { get; init; }
    int PageSize { get; init; }
}