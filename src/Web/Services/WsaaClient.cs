using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace AfipFacturacion.Services
{
    public class WsaaClient
    {
        private readonly string _cuit;
        private readonly X509Certificate2 _certificate;
        private readonly string _service;

        public WsaaClient(string cuit, X509Certificate2 certificate, string service)
        {
            _cuit = cuit;
            _certificate = certificate;
            _service = service;
        }

        public (string token, string sign) Authenticate()
        {
            var loginTicketRequest = CreateLoginTicketRequest();
            var signedTicket = SignLoginTicketRequest(loginTicketRequest);

            var wsaaUrl = "https://wsaa.afip.gov.ar/ws/services/LoginCms";
            using (var client = new System.Net.WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var response = client.UploadString(wsaaUrl, signedTicket);
                return ParseLoginTicketResponse(response);
            }
        }

        private string CreateLoginTicketRequest()
        {
            var uniqueId = DateTimeOffset.Now.ToUnixTimeSeconds();
            var generationTime = DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:ssZ");
            var expirationTime = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:ssZ");

            var xmlRequest = new StringBuilder();
            xmlRequest.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlRequest.Append("<loginTicketRequest version=\"1.0\">");
            xmlRequest.Append("<header>");
            xmlRequest.Append($"<uniqueId>{uniqueId}</uniqueId>");
            xmlRequest.Append($"<generationTime>{generationTime}</generationTime>");
            xmlRequest.Append($"<expirationTime>{expirationTime}</expirationTime>");
            xmlRequest.Append("</header>");
            xmlRequest.Append("<service>").Append(_service).Append("</service>");
            xmlRequest.Append("</loginTicketRequest>");

            return xmlRequest.ToString();
        }

        private string SignLoginTicketRequest(string loginTicketRequest)
        {
            var contentBytes = Encoding.UTF8.GetBytes(loginTicketRequest);
            var signedContent = Sign(contentBytes, _certificate);
            return Convert.ToBase64String(signedContent);
        }

        private byte[] Sign(byte[] content, X509Certificate2 certificate)
        {
            using (var csp = (RSACryptoServiceProvider)certificate.PrivateKey)
            {
                return csp.SignData(content, CryptoConfig.MapNameToOID("SHA256"));
            }
        }

        private (string token, string sign) ParseLoginTicketResponse(string response)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            var token = xmlDoc.SelectSingleNode("//token")?.InnerText;
            var sign = xmlDoc.SelectSingleNode("//sign")?.InnerText;

            return (token, sign);
        }
    }
}
