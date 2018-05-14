using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppIntegracaoCECTiny.Properties;

namespace AppIntegracaoCECTiny
{
    class DadosCeC
    {
        
        private static readonly string Login = Settings.Default.UsuarioCec;
        private static readonly string Senha = Settings.Default.SenhaCec;


        public void ConfirmarRecebimentoPedido2(decimal pedido, bool sincronizarPedido)
        {
            var client = new CeCService.PedidoCompraFornecedoresSoapClient();
            client.ConfirmarRecebimentoPedido(Login, Senha, pedido, sincronizarPedido);
            client.Close();
            //Console.WriteLine(result.mensagem);
        }
        public void BuscarDadosNaCeC2()
        {
            var totalPedidosIntegrados = 0;
            var totalPedidosComErros = 0;
            var ctrTiny = new ControleTiny();
            var result = new CeCService.PedidoCompraFornecedoresSoapClient();
            var res = result.ListarPedidosNaoVisualizados(Login, Senha).pedidos;
            result.Close();

            if (res.Length < 1)
            {
                Console.Clear();
                Console.WriteLine("Nenhum pedido foi encontrado na base de dados da C&C!\n");

                Console.WriteLine("Total de pedidos: {0}.", totalPedidosIntegrados + totalPedidosComErros);
                Console.WriteLine("Total de pedidos integrados: {0}.", totalPedidosIntegrados);
                Console.WriteLine("Total de pedidos com erros na integração: {0}.\n", totalPedidosIntegrados);
             
           


                return;
            }

            foreach (var pedido in res)
            {
                Console.WriteLine("Processando pedido vindo da C&C número: {0}.", pedido.header.numpedidoantigo);
                var listaItens = pedido.itens.Select(pedidoIten => new Item
                    {
                        codigo = pedidoIten.codbarra,
                        descricao = pedidoIten.descricao,
                        quantidade = pedidoIten.qtde,
                        unidade = pedidoIten.unidmaior,
                        valor_unitario = pedidoIten.precounit,
                    })
                    .ToList();

                var clienteTemp = new Cliente
                {
                    bairro = "",
                    atualizar_cliente = "N",
                    cep = pedido.header.filialentrega.endereco.cep,
                    cidade = pedido.header.filialentrega.endereco.municipio,
                    codigo = "",
                    complemento = "",
                    cpf_cnpj = pedido.header.filialentrega.cgccpf,
                    endereco = pedido.header.filialentrega.endereco.ender,
                    fone = pedido.header.filialentrega.fone,
                    ie = pedido.header.filialentrega.ie,
                    nome = pedido.header.filialfatura.nomeloja,
                    nome_fantasia = pedido.header.filialfatura.nomeloja,
                    numero = "",
                    rg = "",
                    tipo_pessoa = "J",
                    uf = pedido.header.filialentrega.endereco.uf
                      
                };

                var dadosTiny = new DadosTiny
                {
                    data_pedido = (pedido.header.dataemissao).ToString("d"),
                    data_prevista = (pedido.header.dataentrega).ToString("d"),
                    cliente = clienteTemp,
                    items = listaItens,
                    nome_transportador = "",
                    forma_pagamento = "",
                    frete_por_conta = "",
                    valor_frete = "",
                    valor_desconto = "",
                    numero_ordem_compra = pedido.header.numpedidoantigo,
                    numero_pedido_ecommerce = pedido.header.numpedidoantigo,
                    situacao = "",
                    obs = "",
                    forma_envio = "",
                    forma_frete = "",
                };

                var ret = ctrTiny.EnviarDadosTiny(dadosTiny);

                if (ret.status.ToUpper() == "ERRO")
                {
                    if (ret.codigo_erro == 2)
                    {
                        Console.WriteLine("Verifique o token usado na conexão com Tiny ERP.");
                    }
                    else
                    {
                        ConfirmarRecebimentoPedido2(pedido.header.numpedidoantigo, true);
                        ++totalPedidosComErros;
                        Console.WriteLine("Pedido vindo da C&C nº: {0}, nao foi integrado pelo motivo abaixo:\n{1}", pedido.header.numpedidoantigo, ret.registros.registro.erros.erro || ret.erro);
                        
                    }

                }
                else
                {
                    try
                    {
                        ConfirmarRecebimentoPedido2(pedido.header.numpedidoantigo, true);
                        ++totalPedidosIntegrados;
                        Console.WriteLine("Pedido integrado com sucesso!\nNumero do pedido gerado pelo tiny: {0}", ret.registros.registro.numero);
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                Console.WriteLine("-----------------------------------------------");

               
            }
            Console.Clear();
            Console.WriteLine("Total de pedidos: {0}.", totalPedidosIntegrados + totalPedidosComErros);
            Console.WriteLine("Total de pedidos integrados: {0}.", totalPedidosIntegrados);
            Console.WriteLine("Total de pedidos com erros na integração: {0}.", totalPedidosIntegrados);
            return;

           
        }

        /*
        internal static void ConvertXml(string dadosEnviados)
        {
            var res = JsonConvert.DeserializeXmlNode(dadosEnviados, "ListarPedidosNaoVisualizadosResult");

            var dados = XDocument.Load(dadosEnviados);
            var nodes = from node in dados.Descendants("ListarPedidosNaoVisualizadosResult") select node;
            dynamic objXml = new ExpandoObject();

            foreach (var filho in nodes.Descendants())
            {
                (objXml as IDictionary<String, object>)[filho.Name.ToString()] = filho.Value.Trim();

                Console.WriteLine(filho.Name.ToString());
                Console.WriteLine(filho.Value.Trim());

            }


        }
        */


   

    }
}
