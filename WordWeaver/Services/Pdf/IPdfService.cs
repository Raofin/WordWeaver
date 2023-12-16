using WordWeaver.Dtos;

namespace WordWeaver.Services.Pdf;

public interface IPdfService
{
    Task<ResponseHelper<byte[]>> PostPdf(long postId);
}