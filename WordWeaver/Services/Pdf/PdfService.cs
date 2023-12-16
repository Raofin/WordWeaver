using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WordWeaver.Data;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Pdf;

public class PdfService(WordWeaverContext context, ILoggerService log, IHtmlGeneratorService htmlGeneratorService, IConverter converter) : IPdfService
{
    public async Task<ResponseHelper<byte[]>> PostPdf(long postId)
    {
        try
        {
            var res = new ResponseHelper<byte[]>();

            // Fetch post data
            var data = await (
                from p in context.Posts
                join u in context.Users on p.UserId equals u.UserId
                where p.PostId == postId
                select new PostPdfDto {
                    PostId = p.PostId,
                    Username = u.Username,
                    Title = p.Title,
                    Description = p.Description,
                    Text = p.Text,
                    PublishedOn = p.CreatedAt,
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                res.StatusCode = HttpStatusCode.NotFound;
                res.Message = "Post not found";
                return res;
            }

            // Create PDF object
            var pdf = new HtmlToPdfDocument() {
                GlobalSettings = new GlobalSettings {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = data.Title,
                },
                Objects = {
                    new ObjectSettings {
                        PagesCount = true,
                        HtmlContent = htmlGeneratorService.BlogPdf(data),
                        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = "" },
                        HeaderSettings = { Line = false },
                        FooterSettings = {
                            FontName = "Arial",
                            FontSize = 7,
                            Line = false,
                            Right = "Page [page] of [toPage]",
                            Center = "Printed on " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt")
                        }
                    }
                }
            };

            return new ResponseHelper<byte[]> {
                StatusCode = HttpStatusCode.OK,
                Data = converter.Convert(pdf)
            };
        }
        catch (Exception ex)
        {
            await log.Error(ex);

            return new ResponseHelper<byte[]> {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
