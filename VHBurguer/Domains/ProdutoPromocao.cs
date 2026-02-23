using System;
using System.Collections.Generic;

namespace VHBurguer.Domains;

public partial class ProdutoPromocao
{
    public int PromocaoId { get; set; }

    public int ProdutoId { get; set; }

    public decimal PrecoAtual { get; set; }

    public virtual Produto Produto { get; set; } = null!;

    public virtual Promocao Promocao { get; set; } = null!;
}
