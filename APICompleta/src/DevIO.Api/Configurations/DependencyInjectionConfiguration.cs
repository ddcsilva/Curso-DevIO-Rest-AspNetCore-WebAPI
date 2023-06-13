using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using DevIO.Business.Services;
using DevIO.Data.Context;
using DevIO.Data.Repository;

namespace DevIO.Api.Configurations;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection ResolveDependencies(this IServiceCollection services)
    {
        services.AddScoped<MeuDbContext>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IEnderecoRepository, EnderecoRepository>();
        
        services.AddScoped<INotificador, Notificador>();
        services.AddScoped<IFornecedorService, FornecedorService>();
        services.AddScoped<IProdutoService, ProdutoService>();

        return services;
    }
}
