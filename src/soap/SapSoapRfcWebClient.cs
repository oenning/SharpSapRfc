﻿using SharpSapRfc.Soap.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace SharpSapRfc.Soap
{
    public class SapSoapRfcWebClient
    {
        private SapSoapRfcDestinationElement destination;
        public SapSoapRfcWebClient(SapSoapRfcDestinationElement destination)
        {
            this.destination = destination;
        }

        public XmlDocument SendRfcRequest(string functionName, XmlDocument soapBody)
        {
            HttpWebRequest request = CreateRequest(this.destination.RfcUrl, functionName, "POST");

            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:rfc:functions"">
               <soapenv:Header/>
               <soapenv:Body>
                     {1}
               </soapenv:Body>
            </soapenv:Envelope>", functionName, soapBody.InnerXml.ToString()));

            using (Stream stream = request.GetRequestStream())
                soapEnvelopeXml.Save(stream);

            return this.ResponseToXml(request);
        }

        public XmlDocument SendWsdlRequest(string functionName)
        {
            HttpWebRequest request = CreateRequest(this.destination.WsdlUrl, functionName, "GET");
            return ResponseToXml(request);
        }

        private XmlDocument ResponseToXml(HttpWebRequest request)
        {
            XmlDocument responseXml = new XmlDocument();
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        responseXml.Load(rd);
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        responseXml.Load(rd);
                }
            }

            return responseXml;
        }

        private HttpWebRequest CreateRequest(string baseUrl, string functionName, string httpMethod)
        {
            string url = string.Format("{0}?sap-client={1}&services={2}", baseUrl, this.destination.Client, functionName);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.Method = httpMethod;
            request.Credentials = new NetworkCredential(this.destination.User, this.destination.Password);
            return request;
        }
    }
}