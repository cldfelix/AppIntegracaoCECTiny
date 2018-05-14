using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AppIntegracaoCECTiny.Properties;
using AppIntegracaoCECTiny.TinyService;
using Newtonsoft.Json;

namespace AppIntegracaoCECTiny
{


     public class NotasFiscais
    {
        [XmlElement("nota_fiscal")]
        public List<nota_fiscal> nota_fiscal { get; set; }
    }


    public class nota_fiscal
    {
        [XmlElement("id")]
        public int id { get; set; }
        [XmlElement("numero")]
        public string numero { get; set; }
    }


    public class ControlerNotas
    {
        private List<string> _numerosDasNotas = new List<string>();
        private static readonly string TokenTiny = Settings.Default.TokenTinyService;

        public static dynamic ObjectToXML(string xml, Type objectType)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            dynamic obj = null;
            try
            {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }

                if (strReader != null)
                {
                    strReader.Close();
                }
            }

            return obj;
        }

        public static void GravarTxt(string nomeDoArquivo, string textoParaGravar)
        {

            //var nomeDoArquivo = "14-05-2018 a 05-05-2018.txt";
            FileInfo aFile = new FileInfo(nomeDoArquivo);

            if (aFile.Exists)
            {
                aFile.Delete();
            }


            using (FileStream fs = aFile.Create())
            {
                Byte[] info =
                    new UTF8Encoding(true).GetBytes(textoParaGravar);

                //Add some information to the file.
                fs.Write(info, 0, info.Length);
            }



        }

        public void BaixarNotasFiscaisTiny(string dataInicial, string dataFinal)
        {
            try
            {
                var nomeDoArquivo = dataInicial.Replace('/', '-') + " a " + dataFinal.Replace('/', '-') + ".txt";

                var client = new tinywsdlPortTypeClient();
                //var ret = client.incluirPedidoServiceAsync(TokenTiny, dadosEnviarTiny, "JSON");
                var ret = client.pesquisarNotasFiscaisService(TokenTiny, "", "", "", "", dataInicial, dataFinal, "7", "", "", "", "XML", "", "", "");
                var jsonObjeto = ObjectToXML(ret, typeof(Retorno));
                client.Close();

                foreach (var nota in (jsonObjeto.notas_fiscais.nota_fiscal))
                {
                    _numerosDasNotas.Add(nota.numero); 
                }


                //GravarTxt(nomeDoArquivo, ret);
                //return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void CriarNotasFiscais()
        {
            foreach (var numerosDasNota in _numerosDasNotas)
            {
                try
                {
                    var client = new tinywsdlPortTypeClient();
                    //var ret = client.incluirPedidoServiceAsync(TokenTiny, dadosEnviarTiny, "JSON");
                    var ret = client.obterNotaFiscalService(TokenTiny, numerosDasNota, "XML");
                    var jsonObjeto = ObjectToXML(ret, typeof(Retorno));
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }




            }
        }

    }
}
