using VHBurguer.Domains;

namespace VHBurguer.Interfaces
{
    public interface IProdutoRepository
    {
        List<Produto> Listar();
        Produto ObterPorID(int id);
        byte[] ObterImagem(int id);
        bool NomeExiste(string nome, int? produtoIDAtual = null);
        void Adicionar(Produto produto, List<int> categoriaIDs);
        void Atualizar(Produto produto, List<int> categoriaIDs);
        void Remover(int id);
    }
}
