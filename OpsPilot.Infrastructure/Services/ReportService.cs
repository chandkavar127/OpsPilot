using ClosedXML.Excel;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly IRepository<Request> _requestRepository;

    public ReportService(IRepository<Request> requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<byte[]> ExportRequestsAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.ListAsync(null, cancellationToken);
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Requests");

        sheet.Cell(1, 1).Value = "Request ID";
        sheet.Cell(1, 2).Value = "Type";
        sheet.Cell(1, 3).Value = "Title";
        sheet.Cell(1, 4).Value = "Employee";
        sheet.Cell(1, 5).Value = "Status";
        sheet.Cell(1, 6).Value = "Requested On";

        var row = 2;
        foreach (var request in requests)
        {
            sheet.Cell(row, 1).Value = request.Id;
            sheet.Cell(row, 2).Value = request.RequestType?.Name ?? string.Empty;
            sheet.Cell(row, 3).Value = request.Title;
            sheet.Cell(row, 4).Value = request.EmployeeProfile?.FullName ?? string.Empty;
            sheet.Cell(row, 5).Value = request.Status.ToString();
            sheet.Cell(row, 6).Value = request.RequestedOnUtc;
            row++;
        }

        sheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}