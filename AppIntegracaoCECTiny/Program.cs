using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntegracaoCECTiny
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder menu = new StringBuilder();
            menu.Append("---------------------------------------");
            menu.Append("\nDigite a opção escolhida:");
            menu.Append("\n1 => Integrar pedidos da C&C no Tiny");
            menu.Append("\n2 => Baixar notas fiscais da Tiny em txt ");
            menu.Append("\nQualquer outra tecla ou numero para sair! ");
            menu.Append("\n---------------------------------------");
            Console.WriteLine(menu);


            while (true)
            {
                
                try
                {
                int numero = Convert.ToInt32(Console.ReadLine());

                    switch (numero)
                    {
                        case 1:
                            var p = new DadosCeC();
                            p.BuscarDadosNaCeC2();
                            break;
                        case 2:
                            Console.WriteLine(numero);
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
           
            }
            Console.ReadKey();
        }
    }
}
