using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
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
        public string tipo_nota { get; set; }
        public string natureza_operacao { get; set; }
        public int regime_tributario { get; set; }
        public int finalidade { get; set; }
        public int serie { get; set; }
        public string numero_ecommerce { get; set; }
        public string data_emissao { get; set; }
        public string data_saida { get; set; }
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
        [XmlArray("itens"), XmlArrayItem(typeof(Item), ElementName = "item")]
        public List<Item> itens { get; set; }
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
    }



    public class ControlerNotas
    {
        private static readonly string TokenTiny = Settings.Default.TokenTinyService;
        private readonly List<int> _numerosDasNotas = new List<int>();
        private string _nomeDoArquivo;

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

        public void GravarTxt(string notasCriadas)
        {

            //var nomeDoArquivo = "14-05-2018 a 05-05-2018.txt";
            //FileInfo aFile = new FileInfo(Directory.GetCurrentDirectory() + "/notasBaixadas/" + this._nomeDoArquivo);
            FileInfo aFile = new FileInfo(this._nomeDoArquivo);

            if (aFile.Exists)
            {
                aFile.Delete();
            }


            using (FileStream fs = aFile.Create())
            {
                Byte[] info =
                    new UTF8Encoding(true).GetBytes(notasCriadas);

                //Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            Console.WriteLine("\nTotal de {0} Notas fiscais gravadas com sucesso!", _numerosDasNotas.Count);
            Console.WriteLine("Pressione qualquer tecla para voltar ao memu principal.");

        }

        public void BaixarNotasFiscaisTiny(string dataInicial, string dataFinal)
        {
            Console.WriteLine("Baixando notas fiscais com DANFE gerado e com datas entre\n{0} e {1}.", dataInicial, dataFinal);
            Console.WriteLine();
            this._nomeDoArquivo = dataInicial.Replace('/', '-') + " a " + dataFinal.Replace('/', '-') + ".txt";

            try
            {
                var client = new tinywsdlPortTypeClient();
                //var ret = client.incluirPedidoServiceAsync(TokenTiny, dadosEnviarTiny, "JSON");
                var ret = client.pesquisarNotasFiscaisService(TokenTiny, "", "", "", "", dataInicial, dataFinal, "7", "", "", "", "XML", "", "", "");
                var jsonObjeto = ObjectToXML(ret, typeof(Retorno));
                client.Close();


                if (jsonObjeto.codigo_erro == 20)
                {
                    Console.Clear();
                    Console.WriteLine("Nenhuma nota fiscal entre as datas {0} e {1} foram encontradas!", dataInicial, dataFinal);
                    return;
                }

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
            var stringNotaFinal = "";
            var qtdDeNoats = 0;

            foreach (var numerosDasNota in _numerosDasNotas)
            {
                ++qtdDeNoats;
                if (qtdDeNoats > 20)
                {
                    Console.WriteLine("\n\nTempo de espera acionado devido a Limites da API...\nO processo vai continuar a depois de 1 minuto!");
                    Console.WriteLine();
                    Thread.Sleep(60000);
                    qtdDeNoats = 0;
                }

                try
                {
                    var client = new tinywsdlPortTypeClient();
                    //var ret = client.incluirPedidoServiceAsync(TokenTiny, dadosEnviarTiny, "JSON");
                    var ret = client.obterNotaFiscalService(TokenTiny, numerosDasNota.ToString(), "XML");
                    var jsonObjeto = ObjectToXML(ret, typeof(Retorno));

                    client.Close();
                    stringNotaFinal = stringNotaFinal + EscreverTxt(jsonObjeto);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
            GravarTxt(stringNotaFinal);
            return;
        }


        private string EscreverTxt(Retorno nota)
        {
            Console.WriteLine("Criando dados txt para nota: {0}", nota.nota_fiscal.numero);
            try
            {
                var txt = new StringBuilder();
                /*
                   1001100000037000000000000092692000000015052018250720180510151015117000110L000417321                                                                                                                    
                   2111111                   0000000003000000256,8000,00000000000.0000.18000000013.87                                     0000                                                                             
                   351045197101110                                                                                                                                                                                        
                   4000000770.40000000138.67000001070.86000000054.08000000770.40000000000.00000000000.00000000000,00000000000.00000000824.4800000000030000011.89                                                          
                   9  
                 */

                var numeropedido = (nota.nota_fiscal.numero_ecommerce).ToString();
                var numpedido = numeropedido.PadLeft(19, '0');
                var dataEmissao = (nota.nota_fiscal.data_emissao).Replace("/", string.Empty);
                var dataVencimento = (nota.nota_fiscal.data_saida).Replace("/", string.Empty); //ver com marcelo
                var numeroNota = (nota.nota_fiscal.numero).ToString().PadLeft(9, '0');
                
                //var cpof = (nota.nota_fiscal.itens[0].cfop).PadLeft(4, ' ');
                var cpof = string.Empty;
                var ufNota = (nota.nota_fiscal.cliente.uf).ToUpper();
                if(ufNota == "SP")
                {
                    cpof = "00005405";

                }
                else
                {
                    cpof = "00006102";
                }

                var cnpj = "13657998000143".PadLeft(14,'0');
                var filler = " ".PadLeft(117, ' ');
                var alicotas = "00,00".PadLeft(5, '0');
                var numeroFci = " ".PadLeft(36, ' ');


                // Header da Nota Fiscal (Tipo 1)
                txt.AppendLine("1" + "9878" + "00000" + " 43" + numpedido + "000000" + dataEmissao + dataVencimento + "1" + cpof + cnpj + "O"+  numeroNota);
                txt.AppendLine();

                // Item da Nota Fiscal (Tipo 2)
                foreach (var iten in nota.nota_fiscal.itens)
                {
                    var qtd = (int) iten.quantidade;
                    txt.AppendLine("2"
                                   + iten.codigo.PadRight(25, ' ')
                                   + qtd.ToString().PadLeft(10, '0')
                                   + iten.valor_unitario.ToString().Replace(".", ",").PadLeft(12, '0')
                                   + alicotas
                                   + (nota.nota_fiscal.valor_ipi).ToString().PadLeft(12, '0')
                                   + alicotas
                                   + (nota.nota_fiscal.valor_icms).ToString().PadLeft(12, '0')
                                   + ' '
                                   + numeroFci
                                   + "0000"
                                   + " ".PadLeft(76, ' ')
                                   );
                }

                // Transportadora (Tipo 3)
                txt.AppendLine("3"
                               + nota.nota_fiscal.transportador.cpf_cnpj.PadRight(14, ' ')
                               + " ".PadLeft(184, ' '));


                // Trailler da Nota Fiscal (Tipo 4)
                txt.AppendLine(("4"
                                + nota.nota_fiscal.base_icms.ToString().PadLeft(12, '0') // Base de Calculo ICMS
                                + nota.nota_fiscal.valor_icms.ToString().PadLeft(12, '0') // Valor do ICMS
                                + nota.nota_fiscal.base_icms_st.ToString()
                                    .PadLeft(12, '0') // Base de Calculo ICMS subst
                                + nota.nota_fiscal.valor_icms_st.ToString().PadLeft(12, '0') // Valor ICMS Substituição
                                + nota.nota_fiscal.valor_nota.ToString().PadLeft(12, '0') // Valor total dos produtos
                                + nota.nota_fiscal.valor_frete.ToString().PadLeft(12, '0') // valor do Frete
                                + nota.nota_fiscal.valor_seguro.ToString().PadLeft(12, '0') // Valor do seguro
                                + nota.nota_fiscal.valor_outras.ToString()
                                    .PadLeft(12, '0') // Outras despesas acessórias
                                + nota.nota_fiscal.valor_ipi.ToString().PadLeft(12, '0') // valor total do IPI
                                + nota.nota_fiscal.valor_nota.ToString().PadLeft(12, '0') // Valor total da nota fiscal
                                + nota.nota_fiscal.quantidade_volumes.ToString().PadLeft(10, '0') // Quantidade
                                + nota.nota_fiscal.peso_bruto.ToString().PadLeft(10, '0') // Peso bruto
                    ).Replace(",", "."));


                // Trailler do arquivo (Fim de arquivo, Tipo 9)
                txt.AppendLine("9".PadRight(199, ' '));
                txt.AppendLine();

                return txt.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



        }

    }
}
