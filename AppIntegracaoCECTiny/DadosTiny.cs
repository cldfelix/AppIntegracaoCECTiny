using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AppIntegracaoCECTiny.Properties;
using Newtonsoft.Json;

/*
 * [16:45, 2/5/2018] Marcelo Garcia GetNinjas: usuário: claudinei@jordano
[16:45, 2/5/2018] Marcelo Garcia GetNinjas: senha: desenvolve
 */

namespace AppIntegracaoCECTiny
{
    public class DadosTiny
    {

        public string data_pedido { get; set; }
        public string data_prevista { get; set; }
        public Cliente cliente { get; set; }


        [XmlArray("itens"), XmlArrayItem(typeof(Item), ElementName = "item")]
        
        public List<Item> items { get; set; }
        [XmlArray("parcelas"), XmlArrayItem(typeof(Parcela), ElementName = "parcela")]
        public List<Parcela> parcelas { get; set; }
        public string nome_transportador { get; set; }
        public string forma_pagamento { get; set; }
        public string frete_por_conta { get; set; }
        public string valor_frete { get; set; }
        public string valor_desconto { get; set; }
        public decimal numero_ordem_compra { get; set; }
        public decimal numero_pedido_ecommerce { get; set; }
        public string situacao { get; set; }
        public string obs { get; set; }
        public string forma_envio { get; set; }
        public string forma_frete { get; set; }

    }

    public class Cliente
    {
        public string atualizar_cliente { get; set; }
        public string codigo { get; set; }
        public string nome { get; set; }
        public string nome_fantasia { get; set; }
        public string tipo_pessoa { get; set; }
        public string cpf_cnpj { get; set; }
        public string ie { get; set; }
        public string rg { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string fone { get; set; }
    }

    public class Item
    {
        public int id_produto { get; set; }
        public string codigo { get; set; }
        public string descricao { get; set; }
        public string unidade { get; set; }
        public decimal quantidade { get; set; }
        public decimal valor_unitario { get; set; }
    }

    public class Parcela
    {
        public string dias { get; set; }
        public string data { get; set; }
        public string valor { get; set; }
        public string obs { get; set; }
        public string forma_pagamento { get; set; }
        public string meio_pagamento { get; set; }
    }

    [XmlRoot("retorno")]
    public class Retorno
    {
        [XmlElement("status_processamento")]
        public int status_processamento { get; set; }

        [XmlElement("status")]
        public string status { get; set; }

        [XmlElement("notas_fiscais")]
        public NotasFiscais notas_fiscais { get; set; }

        [XmlElement("nota_fiscal")]
        public nota_fiscal nota_fiscal { get; set; }

        [XmlElement("registros")]
        public Registros registros { get; set; }

        [XmlElement("codigo_erro")]
        public int codigo_erro { get; set; }


        [XmlElement("erros")]
        public Erros erros { get; set; }

    }

    public class Registros
    {

        [XmlElement("registro")]
        public Registro registro { get; set; }

    }

    public class Registro
    {
        [XmlElement("sequencia")]
        public int sequencia { get; set; }
        [XmlElement("status")]
        public string status { get; set; }

        [XmlElement("codigo_erro")]
        public int codigo_erro { get; set; }


        [XmlElement("erros")]
        public Erros erros { get; set; }

        [XmlElement("id")]
        public long id { get; set; }
        [XmlElement("numero")]
        public long numero { get; set; }
    }

    public class Erros
    {
        [XmlElement("erro")]
        public string erro { get; set; }

    }

    public class Erro
    {
        public string erro { get; set; }
    }



    public class ControleTiny
    {
        private static readonly string TokenTiny = Settings.Default.TokenTinyService;
       


       
        public static string GetXMLFromObject(object o)
        {



            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                var memoryStream = new MemoryStream();
                var streamWriter = new StreamWriter(memoryStream, System.Text.Encoding.UTF8);

                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }

            return sw.ToString();
        }

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

        public dynamic EnviarDadosTiny(DadosTiny dadosEnviarTiny)
        {
            Console.WriteLine("Enviando pedido: {0} na Tiny", dadosEnviarTiny.numero_pedido_ecommerce);
                try
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(DadosTiny));

                    // create a MemoryStream here, we are just working
                    // exclusively in memory
                    System.IO.Stream stream = new System.IO.MemoryStream();

                    // The XmlTextWriter takes a stream and encoding
                    // as one of its constructors
                    System.Xml.XmlTextWriter xtWriter = new System.Xml.XmlTextWriter(stream, Encoding.UTF8);

                    serializer.Serialize(xtWriter, dadosEnviarTiny);

                    xtWriter.Flush();

                    // go back to the beginning of the Stream to read its contents
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    // read back the contents of the stream and supply the encoding
                    System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);

                    var xmlEnvio = reader.ReadToEnd();

                    var client = new TinyService.tinywsdlPortTypeClient();
                    //var ret = client.incluirPedidoServiceAsync(TokenTiny, dadosEnviarTiny, "JSON");
                    var ret = client.incluirPedidoService(TokenTiny, xmlEnvio, "XML");
                    client.Close();

                    return ObjectToXML(ret, typeof(Retorno));
                   
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            

            /*
            XmlSerializer serializer = new XmlSerializer(typeof(DadosTiny));

            System.IO.StringWriter sWriter = new System.IO.StringWriter();

            serializer.Serialize(sWriter, dadosEnviarTiny);
            sWriter.Flush();

            string xmlEnvio = sWriter.ToString();

            //var xmlEnvio = GetXMLFromObject(dadosEnviarTiny);
            var client = new TinyService.tinywsdlPortTypeClient();
            //var ret = client.incluirPedidoServiceAsync(TokenTiny, dadosEnviarTiny, "JSON");
            var ret = client.incluirPedidoService(TokenTiny, xmlEnvio, "XML");
            return ret;
       */
        }
    }

}

    

  