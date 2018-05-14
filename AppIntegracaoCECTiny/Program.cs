using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AppIntegracaoCECTiny
{
    class Program
    {

        static void MenuPrograma()
        {
            while (true)
            {
                Console.Clear();
                Logotipo();
                var menu = new StringBuilder();
                menu.Append("=================================================");
                menu.Append("\nDigite a opção escolhida:");
                menu.Append("\n1 => Integrar pedidos da C&C no Tiny");
                menu.Append("\n2 => Baixar notas fiscais da Tiny em txt ");
                menu.Append("\n0 => Para encerrar e sair do programa. ");
                Console.WriteLine(menu);

                try
                {
                    var numero = Convert.ToInt32(Console.ReadLine());

                    switch (numero)
                    {
                        case 1:
                            var p = new DadosCeC();
                            p.BuscarDadosNaCeC2();
                            break;
                        case 2:
                            Console.Clear();
                            //Console.WriteLine("Digite a data incicial para busca! \nEx. 01/04/2017");
                            //var dataInicial = Convert.ToString(Console.ReadLine());
                            var dataInicial = "01/05/2018";
                            var dataFinal = "15/05/2018";
                            //Console.WriteLine("Digite a data final para busca! \nEx. 18/04/2017");
                            //var dataFinal = Convert.ToString(Console.ReadLine());

                            var matchInicio = Regex.Match(dataInicial, @"^\d{1,2}/\d{1,2}/\d{4}$");
                            var matchFim = Regex.Match(dataFinal, @"^\d{1,2}/\d{1,2}/\d{4}$");
                            if (matchInicio.Success && matchFim.Success )
                            {
                                var t = new ControlerNotas();
                                t.BaixarNotasFiscaisTiny(dataInicial, dataFinal);
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Data com formato inválido!");
                            }
                            
                            break;
                        case 0:
                            Console.Clear();
                            Console.WriteLine("\n\nSaindo do programa..............!");
                            Thread.Sleep(1500);
                            Environment.Exit(1);

                            break;
                        default:
                            Console.WriteLine(numero);
                            return;
                    }

                }
                catch (Exception e)
                {
                    return;
                }

                Console.ReadKey();
            }
        }


        static void Logotipo()
        {
            var logo = new StringBuilder();
            logo.Append("---------------------------------------------------\n");
            logo.Append("-             Integração desenvolvida por:        -\n");
            logo.Append("-                  Claudinei Felix                -\n");
            logo.Append("-      Contato: claudinei.felix@outlook.com       -\n");
            logo.Append("-      Telefone: (19) 9954-60517                  -\n");
            logo.Append("---------------------------------------------------\n");

            Console.WriteLine(logo.ToString());
        }


        static void Main(string[] args)
        {
            while (true)
            {
                MenuPrograma();
                
            }
        }
    }
}
