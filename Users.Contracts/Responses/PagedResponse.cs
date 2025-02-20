﻿namespace Users.Contracts.Responses;

public class PagedResponse<TResponse>
{
    public required IEnumerable<TResponse> Users { get; init; } = Enumerable.Empty<TResponse>();
    public required int PageSize { get; init; }
    public required int Page { get; init; }

    public required int Total { get; init; }
    public bool HasNextPage => Total > (Page * PageSize);

}
