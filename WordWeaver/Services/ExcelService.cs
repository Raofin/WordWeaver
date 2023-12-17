using ClosedXML.Excel;
using System.Net;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Services;

public class ExcelService(ILoggerService log) : IExcelService
{
    public async Task<ResponseHelper<byte[]>> UsersListExcel(List<UserListDto> users)
    {
        try
        {
            var res = new ResponseHelper<byte[]>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");

                // Add headers
                worksheet.Cell(1, 1).Value = "User ID";
                worksheet.Cell(1, 2).Value = "Username";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Joined At";
                worksheet.Cell(1, 5).Value = "Updated At";
                worksheet.Cell(1, 6).Value = "Is Active";
                worksheet.Cell(1, 7).Value = "Profile ID";
                worksheet.Cell(1, 8).Value = "Full Name";
                worksheet.Cell(1, 9).Value = "Bio";
                worksheet.Cell(1, 10).Value = "Birthday";
                worksheet.Cell(1, 11).Value = "Country";
                worksheet.Cell(1, 12).Value = "Phone";
                worksheet.Cell(1, 13).Value = "Website";
                worksheet.Cell(1, 14).Value = "Joined Date";

                // Add headers for dynamic columns (Socials and UserRoles)
                worksheet.Cell(1, 15).Value = "Social ID";
                worksheet.Cell(1, 16).Value = "Social Name";
                worksheet.Cell(1, 17).Value = "Social URL";

                worksheet.Cell(1, 18).Value = "Role ID";
                worksheet.Cell(1, 19).Value = "Role Name";

                // Add more headers as needed

                // Add data
                var row = 2;
                foreach (var user in users)
                {
                    worksheet.Cell(row, 1).Value = user.UserId;
                    worksheet.Cell(row, 2).Value = user.Username;
                    worksheet.Cell(row, 3).Value = user.Email;
                    worksheet.Cell(row, 4).Value = user.JoinedAt;
                    worksheet.Cell(row, 5).Value = user.UpdatedAt;
                    //worksheet.Cell(row, 6).Value = user.IsActive;

                    // Nested objects (ProfileDto)
                    if (user.UserDetails != null)
                    {
                        worksheet.Cell(row, 7).Value = user.UserDetails.ProfileId;
                        worksheet.Cell(row, 8).Value = user.UserDetails.FullName;
                        worksheet.Cell(row, 9).Value = user.UserDetails.Bio;
                        worksheet.Cell(row, 10).Value = user.UserDetails.Birthday;
                        worksheet.Cell(row, 11).Value = user.UserDetails.Country;
                        worksheet.Cell(row, 12).Value = user.UserDetails.Phone;
                        worksheet.Cell(row, 13).Value = user.UserDetails.Website;
                        worksheet.Cell(row, 14).Value = user.UserDetails.JoinedDate;

                        // Dynamic columns for Socials
                        if (user.UserDetails.Socials != null)
                        {
                            var socialCol = 15;
                            foreach (var social in user.UserDetails.Socials)
                            {
                                worksheet.Cell(row, socialCol++).Value = social.SocialId;
                                worksheet.Cell(row, socialCol++).Value = social.SocialName;
                                worksheet.Cell(row, socialCol++).Value = social.SocialUrl;
                            }
                        }
                        // Fill empty columns for Socials if there are fewer items
                        FillEmptyColumns(worksheet, row, 15, 3);

                        // Dynamic columns for UserRoles
                        if (user.UserRoles != null)
                        {
                            var roleCol = 18;
                            foreach (var userRole in user.UserRoles)
                            {
                                worksheet.Cell(row, roleCol++).Value = userRole.RoleId;
                                worksheet.Cell(row, roleCol++).Value = userRole.RoleName;
                            }
                        }
                        // Fill empty columns for UserRoles if there are fewer items
                        FillEmptyColumns(worksheet, row, 18, 2);
                    }

                    row++;
                }

                // Auto-adjust column widths for better appearance
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    res.StatusCode = HttpStatusCode.OK;
                    res.Data = stream.ToArray();
                    return res;
                }
            }
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

    private void FillEmptyColumns(IXLWorksheet worksheet, int row, int startCol, int columnCount)
    {
        for (var i = 0; i < columnCount; i++)
        {
            worksheet.Cell(row, startCol + i).Value = "";
        }
    }
}
