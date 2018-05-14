/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AppIntegracaoCECTiny.CeCService;
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
        public string tipo_nota { get; set; }
        public string natureza_operacao { get; set; }
        public int regime_tributario { get; set; }
        public int finalidade { get; set; }
        public int serie { get; set; }
        public int numero { get; set; }
        public string numero_ecommerce { get; set; }
        public DateTime data_emissao { get; set; }
        public DateTime data_saida { get; set; }
        public Cliente cliente { get; set; }
        public EnderecoEntrega endereco_entrega { get; set; }
        public decimal base_icms { get; set; }
        public decimal valor_icms { get; set; }
        public decimal base_icms_st { get; set; }
        public decimal valor_icms_st { get; set; }
        public decimal valor_servicos { get; set; }
        public decimal valor_produtos { get; set; }
        public decimal valor_frete { get; set; }
        public decimal valor_seguro { get; set; }
        public decimal valor_outras { get; set; }
        public decimal valor_ipi { get; set; }
        public decimal valor_issqn { get; set; }
        public decimal valor_nota { get; set; }
        public decimal valor_desconto { get; set; }
        public decimal valor_faturado { get; set; }
        public string frete_por_conta { get; set; }
        public Transportador transportador { get; set; }

        public string placa { get; set; }
        public string uf_placa { get; set; }
        public int quantidade_volumes { get; set; }
        public string especie_volumes { get; set; }
        public string marca_volumes { get; set; }
        public string numero_volumes { get; set; }
        public decimal peso_bruto { get; set; }
        public decimal peso_liquido { get; set; }
        public string codigo_rastreamento { get; set; }
        public string url_rastreamento { get; set; }
        public string forma_pagamento { get; set; }
        public string meio_pagamento { get; set; }
        public List<Parcela> parcelas { get; set; }
        public int id_vendedor { get; set; }
        public string nome_vendedor { get; set; }
        public int situacao { get; set; }
        public string descricao_situacao { get; set; }
        public string obs { get; set; }
        public string chave_acesso { get; set; }
    }

    public class Transportador
    {
        public string nome { get; set; }
        public string cpf_cnpj { get; set; }
        public string ie { get; set; }
        public string endereco { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
    }

    public class EnderecoEntrega
    {
        public string tipo_pessoa { get; set; }
        public string cpf_cnpj { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string fone { get; set; }
        public string nome_destinatario { get; set; }
        public List<Item> itens { get; set; }
    }



    [XmlRoot("retorno")]
    public class NotaCompleta
    {
        public int status_processamento { get; set; }
        public int codigo_erro { get; set; }
        public string status { get; set; }
        public List<Erros> erros { get; set; }

    }



    public class ControlerNotas
    {
        private List<int> _numerosDasNotas = new List<int>();
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
                    _numerosDasNotas.Add(nota.id); 
                }

                CriarNotasFiscais();
                return;
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
                    var ret = client.obterNotaFiscalService(TokenTiny, numerosDasNota.ToString(), "XML");
                    var jsonObjeto = ObjectToXML(ret, typeof(nota_fiscal));
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
*/