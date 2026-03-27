namespace OpsPilot.Application.Common.Interfaces;

public interface IReportService
{
    Task<byte[]> ExportRequestsAsync(CancellationToken cancellationToken = default);
}
