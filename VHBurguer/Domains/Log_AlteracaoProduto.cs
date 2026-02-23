using System;
using System.Collections.Generic;

namespace VHBurguer.Domains;

public partial class Log_AlteracaoProduto
{
    public int Log_AlteracaoProdutoId { get; set; }

    public DateTime DataAlteracao { get; set; }

    public string? NomeAnterior { get; set; }

    public decimal? PrecoAnterior { get; set; }

    public int? ProdutoId { get; set; }

    public virtual Produto? Produto { get; set; }
}
