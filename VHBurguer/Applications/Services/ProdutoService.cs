using VHBurguer.Applications.Conversoes;
using VHBurguer.Applications.Regras;
using VHBurguer.Domains;
using VHBurguer.DTOs.ProdutoDTO;
using VHBurguer.Exceptions;
using VHBurguer.Interfaces;

namespace VHBurguer.Applications.Services
{
    public class ProdutoService
    {
        private readonly IProdutoRepository _repository;

        public ProdutoService(IProdutoRepository repository)
        {
            _repository = repository;
        }

        // Para cada produto que veio do banco
        // Crie um DTO só com o que a requisição/front precisa.
        public List<LerProdutoDTO> Listar()
        {
            List<Produto> produtos = _repository.Listar();

            // SELECT percorre cada Produto e transforma em DTO -> LerProdutoDto
            List<LerProdutoDTO> produtosDto =
                produtos.Select(ProdutoParaDTO.ConverterParaDTO).ToList();

            return produtosDto;
        }

        public LerProdutoDTO ObterPorID(int id)
        {
            Produto produto = _repository.ObterPorID(id);

            if (produto == null)
            {
                throw new DomainException("Produto não encontrado");
            }

            // converte o produto encontrado para DTO e devolve
            return ProdutoParaDTO.ConverterParaDTO(produto);
        }

        private static void ValidarCadastro(CriarProdutoDTO produtoDto)
        {
            if (string.IsNullOrWhiteSpace(produtoDto.Nome))
            {
                throw new DomainException("Nome é obrigatório.");
            }

            if (produtoDto.Preco < 0)
            {
                throw new DomainException("Preço deve ser maior que zero.");
            }

            if (string.IsNullOrWhiteSpace(produtoDto.Descricao))
            {
                throw new DomainException("Descrição é obrigatória.");
            }

            if (produtoDto.Imagem == null || produtoDto.Imagem.Length == 0)
            {
                throw new DomainException("Imagem é obrigatória.");
            }

            if (produtoDto.CategoriaIDs == null || produtoDto.CategoriaIDs.Count == 0)
            {
                throw new DomainException("Produto deve ter ao menos uma categoria.");
            }
        }

        public byte[] ObterImagem(int id)
        {
            byte[] imagem = _repository.ObterImagem(id);

            if (imagem == null || imagem.Length == 0)
            {
                throw new DomainException("Imagem não encontrada");
            }

            return imagem;
        }

        public LerProdutoDTO Adicionar(CriarProdutoDTO produtoDTO, int usuarioID)
        {
            ValidarCadastro(produtoDTO);

            if (_repository.NomeExiste(produtoDTO.Nome))
            {
                throw new DomainException("Produto já existente");
            }

            Produto produto = new Produto
            {
                Nome = produtoDTO.Nome,
                Preco = produtoDTO.Preco,
                Descricao = produtoDTO.Descricao,
                Imagem = ImagemParaBytes.ConverterImagem(produtoDTO.Imagem),
                StatusProduto = true,
                UsuarioID = usuarioID
            };

            _repository.Adicionar(produto, produtoDTO.CategoriaIDs);

            return ProdutoParaDTO.ConverterParaDTO(produto);
        }

        public LerProdutoDTO Atualizar(int id, AtualizarProdutoDTO produtoDTO)
        {
            HorarioAlteracaoProduto.ValidarHorario();

            Produto produtoBanco = _repository.ObterPorID(id);

            if (produtoBanco == null)
            {
                throw new DomainException("Produto não encontrado.");
            }

            // produtoIdAtual: -> dois pontos serve para passar o valor do parametro
            if (_repository.NomeExiste(produtoDTO.Nome, produtoIDAtual: id))
            {
                throw new DomainException("Já existe outro produto com esse nome.");
            }

            if (produtoDTO.CategoriaIDs == null || produtoDTO.CategoriaIDs.Count == 0)
            {
                throw new DomainException("Produto deve ter ao menos uma categoria.");
            }

            if (produtoDTO.Preco < 0)
            {
                throw new DomainException("Preço deve ser maior que zero.");
            }

            produtoBanco.Nome = produtoDTO.Nome;
            produtoBanco.Preco = produtoDTO.Preco;
            produtoBanco.Descricao = produtoDTO.Descricao;

            if (produtoDTO.Imagem != null && produtoDTO.Imagem.Length > 0)
            {
                produtoBanco.Imagem = ImagemParaBytes.ConverterImagem(produtoDTO.Imagem);
            }

            if (produtoDTO.StatusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produtoDTO.StatusProduto.Value;
            }

            _repository.Atualizar(produtoBanco, produtoDTO.CategoriaIDs);

            return ProdutoParaDTO.ConverterParaDTO(produtoBanco);

        }

        public void Remover(int id)
        {
            HorarioAlteracaoProduto.ValidarHorario();

            Produto produto = _repository.ObterPorID(id);

            if (produto == null)
            {
                throw new DomainException("Produto não encontrado.");
            }

            _repository.Remover(id);
        }

        internal LerProdutoDTO ObterPorID(int id)
        {
            throw new NotImplementedException();
        }
    }
}