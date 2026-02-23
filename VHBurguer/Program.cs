using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VHBurguer.Applications.Services;
using VHBurguer.Contexts;
using VHBurguer.Interfaces;
using VHBurguer.Repositories;
using VHBurguer.Applications.Autenticacao;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// chamar nossa conexão com o banco aqui na program
builder.Services.AddDbContext<VH_BurguerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Usuário
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

// Produto
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();

// Jwt
builder.Services.AddScoped<GeradorTokenJwt>();
builder.Services.AddScoped<AutenticacaoService>();


// Configura o sistema de autenticação da aplicação.
// Aqui estamos dizendo que o tipo de autenticação padrão será Jwt Bearer.
// Ou seja: a API vai esperar receber um Token Jwt nas requisições.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    // Adiciona o suporte para autenticação usando Jwt.
    .AddJwtBearer(options =>
    {
        // Lê a chave secreta definIda no appsettings.json.
        // Essa chave é usada para ASSINAR o token quando ele é gerado
        // e também para ValidAR se o token recebIdo é verdadeiro.
        var chave = builder.Configuration["Jwt:Key"]!;

        // Quem emitiu o token (ex: nome da sua aplicação).
        // Serve para evitar aceitar tokens de outro sistema.
        var issuer = builder.Configuration["Jwt:Issuer"]!;

        // Para quem o token foi criado (normalmente o frontend ou a própria API).
        // Também ajuda a garantir que o token pertence ao seu sistema.
        var audience = builder.Configuration["Jwt:Audience"]!;

        // Define as regras que serão usadas para Validar o token recebIdo.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Verifica se o emissor do token é válIdo
            // (se bate com o issuer configurado).
            ValidateIssuer = true,

            // Verifica se o destinatário do token é válIdo
            // (se bate com o audience configurado).
            ValidateAudience = true,

            // Verifica se o token ainda está dentro do prazo de Validade.
            // Se já expirou, a requisição será negada.
            ValidateLifetime = true,

            // Verifica se a assinatura do token é válIda.
            // Isso garante que o token não foi alterado.
            ValidateIssuerSigningKey = true,

            // Define qual emissor é consIderado válIdo.
            ValidIssuer = issuer,

            // Define qual audience é consIderado válIdo.
            ValidAudience = audience,

            // Define qual chave será usada para Validar a assinatura do token.
            // A mesma chave usada na geração do Jwt deve estar aqui.
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(chave)
            )
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();