using WordWeaver.Dtos;

namespace WordWeaver.Services.Pdf
{
    public interface IHtmlGeneratorService
    {
        string BlogPdf(PostPdfDto dto);
    }
}