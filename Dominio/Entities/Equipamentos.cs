using Dominio.Entities.Enums;

namespace Dominio.Entities
{
    public class Equipamento
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }

        public string? CodigoPatrimonio { get; private set; }

        public string? FotoUrl { get; private set; }

        public OrigemEquipamento Origem { get; private set; }

        public StatusEquipamento Status { get; private set; }

        // campos para quando a origem for equipamento Locado
        public string? FornecedorLocacao { get; private set; }
        public DateTime? DataInicioLocacao { get; private set; }

        public DateTime? DataFimLocacao { get; private set; }

        public int? ObraId { get; private set; }
        public Obra? Obra { get; private set; }

        public DateTime CriadoEm { get; private set; }

        private Equipamento() { }

        public Equipamento(string nome, OrigemEquipamento origem, string? codigoPatrimonio = null, string? fotoUrl = null, string? fornecedorLocacao = null, DateTime? dataInicioLocacao = null, DateTime? dataFimLocacao = null)

        {
            CriadoEm = DateTime.Now;
            AtualizarDados(nome, origem, codigoPatrimonio, fotoUrl, fornecedorLocacao, dataInicioLocacao, dataFimLocacao);
            Status = StatusEquipamento.Disponivel;
        }


        public void AtualizarDados(string nome, OrigemEquipamento origem, string? codigoPatrimonio, string? fotoUrl, string? fornecedorLocacao, DateTime? dataInicioLocacao, DateTime? dataFimLocacao)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("O nome do equipamento é obrigatório", nameof(nome));
            }

            if (origem == OrigemEquipamento.Locado)
            {
                if (dataInicioLocacao.HasValue && dataFimLocacao.HasValue && dataFimLocacao < dataInicioLocacao)
                {
                    throw new ArgumentException("A data final não pode anteceder a data de inicial");
                }
            }
            else
            {
                fornecedorLocacao = null;
                dataInicioLocacao = null;
                dataFimLocacao = null;
            }

            Nome = nome.Trim();
            Origem = origem;
            CodigoPatrimonio = string.IsNullOrWhiteSpace(codigoPatrimonio) ? null : codigoPatrimonio.Trim();
            FotoUrl = string.IsNullOrWhiteSpace(fotoUrl) ? null : fotoUrl.Trim();
            FornecedorLocacao = string.IsNullOrWhiteSpace(fornecedorLocacao) ? null : fornecedorLocacao.Trim();
            DataInicioLocacao = dataInicioLocacao;
            DataFimLocacao = dataFimLocacao;

        }

        public void AlterarStatus(StatusEquipamento novoStatus)
        {
            if (novoStatus == StatusEquipamento.EmUso && !ObraId.HasValue)
            {
                throw new InvalidOperationException("Não é possível marcar sem haver vinculo com uma obra");
            }

            if (novoStatus == StatusEquipamento.ComDefeito && ObraId.HasValue)
            {
                ObraId = null;
                Obra = null;
            }

            Status = novoStatus;
        }

        public void AlocarNaObra(int obraId)
        {
            if (Status == StatusEquipamento.ComDefeito)
            {
                throw new InvalidOperationException("Não é possivel alocar a uma obra um equipamento com defeito");
            }

            if (Status == StatusEquipamento.Devolvido)
            {
                throw new InvalidOperationException("Equipamento locado já foi devolvido ao fornecedor");
            }

            ObraId = obraId;
            Status = StatusEquipamento.EmUso;

        }

        public void DesalocarDaObra(bool marcarComoDevolvidoSeLocado = false)
        {
            ObraId = null;
            Obra = null;
            if (Status == StatusEquipamento.ComDefeito)
                return;
            

            if (Origem == OrigemEquipamento.Locado && marcarComoDevolvidoSeLocado)
                Status = StatusEquipamento.Devolvido;
                else Status = StatusEquipamento.Disponivel;
            
        }
        


    }
}