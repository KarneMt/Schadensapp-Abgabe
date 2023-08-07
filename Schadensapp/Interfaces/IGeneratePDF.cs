using Schadensapp.Models;
using Schadensapp.Services.Mail;
using Schadensapp.Services.Mail.Model;

namespace Schadensapp.Interfaces
{
    public interface IGeneratePDF
    {
        public MemoryStream GeneratePDF(PDFModel model);
        string GenerateTemplate(PDFModel model);
    }
}
