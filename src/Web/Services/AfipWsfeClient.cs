using System;
using System.ServiceModel;
using System.Threading.Tasks;
using AfipFacturacion.Wsfe; // Namespace generado al agregar la referencia al servicio WSFE

namespace AfipFacturacion.Services
{
    public class AfipWsfeClient
    {
        private readonly ServiceSoapClient _client;
        private readonly string _cuit;
        private readonly string _token;
        private readonly string _sign;

        public AfipWsfeClient(string cuit, string token, string sign, bool isProduction = false)
        {
            _cuit = cuit;
            _token = token;
            _sign = sign;

            var url = isProduction
                ? "https://servicios1.afip.gov.ar/wsfev1/service.asmx"
                : "https://wswhomo.afip.gov.ar/wsfev1/service.asmx";

            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.MaxReceivedMessageSize = 2000000;

            var endpoint = new EndpointAddress(url);
            _client = new ServiceSoapClient(binding, endpoint);
        }

        public async Task<int> ObtenerUltimoNumeroFactura(int puntoVenta, int tipoComprobante)
        {
            var auth = new FEAuthRequest
            {
                Cuit = Convert.ToInt64(_cuit),
                Sign = _sign,
                Token = _token
            };

            var response = await _client.FECompUltimoAutorizadoAsync(auth, puntoVenta, tipoComprobante);
            return response.Body.FECompUltimoAutorizadoResult.CbteNro;
        }
    }
}
