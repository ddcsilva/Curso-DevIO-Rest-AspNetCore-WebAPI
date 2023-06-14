using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Route("api/produtos")]
public class ProdutosController : MainController
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IProdutoService _produtoService;
    private readonly IMapper _mapper;

    public ProdutosController(INotificador notificador,
                             IProdutoRepository produtoRepository,
                             IProdutoService produtoService,
                             IMapper mapper) : base(notificador)
    {
        _produtoRepository = produtoRepository;
        _produtoService = produtoService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
    {
        return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
    {
        var produtoViewModel = await ObterProduto(id);

        if (produtoViewModel == null) return NotFound();

        return produtoViewModel;
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
        if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
        {
            return CustomResponse(produtoViewModel);
        }

        produtoViewModel.Imagem = imagemNome;
        await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

        return CustomResponse(produtoViewModel);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProdutoViewModel>> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
    {
        if (id != produtoViewModel.Id)
        {
            NotificarErro("O id informado não é o mesmo que foi passado na query");
            return CustomResponse(produtoViewModel);
        }

        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _produtoService.Atualizar(_mapper.Map<Produto>(produtoViewModel));

        return CustomResponse(produtoViewModel);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
    {
        var produtoViewModel = await ObterProduto(id);

        if (produtoViewModel == null) return NotFound();

        await _produtoService.Remover(id);

        return CustomResponse(produtoViewModel);
    }

    private async Task<ProdutoViewModel> ObterProduto(Guid id)
    {
        return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
    }

    private bool UploadArquivo(string arquivo, string nomeImagem)
    {
        if (string.IsNullOrEmpty(arquivo))
        {
            NotificarErro("Forneça uma imagem para este produto!");
            return false;
        }

        var imageDataByteArray = Convert.FromBase64String(arquivo);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", nomeImagem);

        if (System.IO.File.Exists(filePath))
        {
            NotificarErro("Já existe um arquivo com este nome!");
            return false;
        }

        System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

        return true;
    }
}