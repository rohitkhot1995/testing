using System;
using System.IO;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Enums;

namespace Gaming.Predictor.Interfaces.AWS
{
    public interface IAWS
    {
        Task<String> Get(String fileName);
        Task<byte[]> GetImage(String fileName);

        Task<bool> Set(String fileName, Object content, bool serialize);
        Task MoveToS3(String fromServer, String toS3Path, string filename);
        Task<bool> Set(String bucket, String fileName, Object content, bool serialize);
        Task<bool> SetImage(String fileName, Stream image, bool downloadable);
        Task Log(HTTPLog logMessage);
        Task Debug(String message);

        Task<bool> Has(String fileName);
        Task<bool> Remove(String fileName);

        Task<String> Environment();

        //Task<object> TextEmail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml);
        //Task<object> AttachmentEmail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml, byte[] attachment = null);
        //Task<object> SMTPEmail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml);

        Task<bool> SendSESMail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml, byte[] attachment = null);

        Task<String> ReadMessage(String queueURL);
        Task<String> DeleteMessage(String receiptHandle, String queueURL);
        Task<bool> SendSNSAlert(String subject, String message);

        Task<bool> WriteImageOnS3(Stream imageStream, String extension, String keyValue);
        bool WriteS3Asset(String fileName, MimeType type, Object content, byte[] imageBytes = null);
        bool WriteS3Asset(String fileName, string extension, MimeType type, Object content, byte[] imageBytes = null);

    }
}