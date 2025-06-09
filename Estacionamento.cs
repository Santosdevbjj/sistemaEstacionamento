using System;
using System.Collections.Generic;
using System.Linq; // Necessário para o método .Any() e ToUpper()

namespace DesafioFundamentos.Models
{
    /// <summary>
    /// Representa a lógica de um sistema de estacionamento.
    /// Gerencia veículos estacionados, preços e operações de adicionar/remover.
    /// </summary>
    public class Estacionamento
    {
        // Campos privados para armazenar os preços e a lista de veículos.
        // O modificador 'private' garante que esses campos só podem ser acessados dentro desta classe.
        private decimal precoInicial = 0;
        private decimal precoPorHora = 0;
        private List<string> veiculos = new List<string>(); // Lista genérica para armazenar as placas dos veículos (strings).

        /// <summary>
        /// Construtor da classe Estacionamento.
        /// É chamado quando uma nova instância de Estacionamento é criada.
        /// </summary>
        /// <param name="precoInicial">O preço fixo a ser cobrado ao entrar no estacionamento.</param>
        /// <param name="precoPorHora">O preço a ser cobrado por cada hora que o veículo permanecer estacionado.</param>
        public Estacionamento(decimal precoInicial, decimal precoPorHora)
        {
            // Atribui os valores passados como parâmetro aos campos privados da classe.
            this.precoInicial = precoInicial;
            this.precoPorHora = precoPorHora;
        }

        /// <summary>
        /// Adiciona um novo veículo à lista de veículos estacionados.
        /// Solicita a placa do veículo ao usuário via console.
        /// </summary>
        public void AdicionarVeiculo()
        {
            Console.WriteLine("\n--- Adicionar Veículo ---");
            Console.Write("Digite a placa do veículo para estacionar: ");
            // Lê a linha digitada pelo usuário no console e armazena na variável 'placa'.
            string placa = Console.ReadLine(); 

            // Validação simples para garantir que a placa não está vazia ou contém apenas espaços em branco.
            if (string.IsNullOrWhiteSpace(placa))
            {
                Console.WriteLine("Placa inválida. Por favor, digite uma placa.");
                return; // Sai do método sem adicionar o veículo se a placa for inválida.
            }

            // Opcional: Verificação para evitar a adição de placas duplicadas.
            // .Any() verifica se existe algum elemento na lista que satisfaz a condição.
            // .Equals(placa, StringComparison.OrdinalIgnoreCase) compara as strings ignorando maiúsculas/minúsculas.
            if (veiculos.Any(v => v.Equals(placa, StringComparison.OrdinalIgnoreCase))) 
            {
                Console.WriteLine($"O veículo com a placa '{placa.ToUpper()}' já está estacionado.");
                return; // Sai do método se a placa já estiver na lista.
            }

            // Adiciona a placa à lista de veículos. 
            // .ToUpper() converte a placa para maiúsculas para padronização no armazenamento.
            veiculos.Add(placa.ToUpper()); 
            Console.WriteLine($"Veículo com placa '{placa.ToUpper()}' estacionado com sucesso!");
        }

        /// <summary>
        /// Remove um veículo da lista de veículos estacionados e calcula o valor a ser pago.
        /// Solicita a placa do veículo e a quantidade de horas que permaneceu estacionado.
        /// </summary>
        public void RemoverVeiculo()
        {
            Console.WriteLine("\n--- Remover Veículo ---");
            Console.Write("Digite a placa do veículo para remover: ");

            // Lê a placa digitada pelo usuário para remover.
            string placa = Console.ReadLine(); 

            // Validação simples para garantir que a placa a ser removida não está vazia.
            if (string.IsNullOrWhiteSpace(placa))
            {
                Console.WriteLine("Placa inválida. Por favor, digite a placa do veículo a ser removido.");
                return; // Sai do método.
            }

            // Verifica se o veículo existe na lista. 
            // .Any() com StringComparison.OrdinalIgnoreCase para uma busca que não diferencia maiúsculas de minúsculas.
            if (veiculos.Any(x => x.Equals(placa, StringComparison.OrdinalIgnoreCase))) 
            {
                Console.Write("Digite a quantidade de horas que o veículo permaneceu estacionado: ");
                string horasTexto = Console.ReadLine(); // Lê a entrada do usuário como texto.
                int horas = 0;
                decimal valorTotal = 0;

                // Tenta converter a string 'horasTexto' para um número inteiro 'horas'.
                // TryParse é seguro, pois não lança exceção se a conversão falhar, apenas retorna 'false'.
                if (int.TryParse(horasTexto, out horas))
                {
                    // Realiza o cálculo do valor total.
                    // O preço inicial é fixo, e o preço por hora é multiplicado pela quantidade de horas.
                    valorTotal = precoInicial + (precoPorHora * horas);

                    // Remove a placa da lista de veículos.
                    // .First() encontra a primeira ocorrência que corresponde à condição.
                    // .Remove() remove esse elemento específico da lista.
                    veiculos.Remove(veiculos.First(x => x.Equals(placa, StringComparison.OrdinalIgnoreCase)));

                    // Exibe a mensagem de remoção e o valor total.
                    // :F2 formata o número decimal para duas casas decimais (ex: 25.00).
                    Console.WriteLine($"O veículo '{placa.ToUpper()}' foi removido e o preço total foi de: R$ {valorTotal:F2}"); 
                }
                else
                {
                    // Mensagem de erro se a entrada de horas não for um número válido.
                    Console.WriteLine("Quantidade de horas inválida. Por favor, digite um número inteiro.");
                }
            }
            else
            {
                // Mensagem se a placa digitada não for encontrada na lista de veículos estacionados.
                Console.WriteLine("Desculpe, esse veículo não está estacionado aqui. Confira se digitou a placa corretamente.");
            }
        }

        /// <summary>
        /// Lista todos os veículos atualmente estacionados.
        /// Informa se não há veículos ou exibe a placa de cada um.
        /// </summary>
        public void ListarVeiculos()
        {
            Console.WriteLine("\n--- Veículos Estacionados ---");
            // Verifica se há algum veículo na lista usando o método .Any() do LINQ.
            // É mais semântico do que verificar 'veiculos.Count > 0'.
            if (veiculos.Any()) 
            {
                Console.WriteLine("Os veículos estacionados são:");
                // Loop 'for' para iterar sobre a lista de veículos.
                // 'i' representa o índice do veículo (começando em 0).
                for (int i = 0; i < veiculos.Count; i++)
                {
                    // Exibe o número da "vaga" (índice + 1 para começar em 1) e a placa do veículo.
                    Console.WriteLine($"Vaga {i + 1}: {veiculos[i]}"); 
                }
                // Outra forma de iterar, usando 'foreach' (mais simples para percorrer todos os itens sem precisar do índice):
                // int contador = 1;
                // foreach (string veiculo in veiculos)
                // {
                //     Console.WriteLine($"Vaga {contador}: {veiculo}");
                //     contador++;
                // }
            }
            else
            {
                // Mensagem exibida se a lista de veículos estiver vazia.
                Console.WriteLine("Não há veículos estacionados.");
            }
        }
    }
}
 





