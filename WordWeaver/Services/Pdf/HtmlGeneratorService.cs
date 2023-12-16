using System.Text;
using WordWeaver.Dtos;

namespace WordWeaver.Services.Pdf;

public class HtmlGeneratorService : IHtmlGeneratorService
{
    public string BlogPdf(PostPdfDto dto)
    {
        var sb = new StringBuilder();

        sb.Append(@$"
            <body style='font-family: 'Arial', sans-serif; margin: 20px;'>
                <div style='text-align: center;'>
                    <h1 style='color: #333;'>{dto.Title}</h1>
                    <p style='color: #555;'>By {dto.Username} - Published on {dto.PublishedOn}</p>
                </div>
                <div style='margin-top: 20px;'>
                    <p style='color: #777;'>{dto.Description}</p>
                </div>
                <div style='margin-top: 20px;'>
                    {dto.Text}
                </div>
            </body>");

        return sb.ToString();
    }
}
