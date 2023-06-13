using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Route("api/fornecedores")]
public class FornecedoresController : MainController
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IFornecedorService _fornecedorService;
    private readonly IMapper _mapper;

    public FornecedoresController(IFornecedorRepository fornecedorRepository, 
                                  IFornecedorService fornecedorService, 
                                  IMapper mapper)
    {
        _fornecedorRepository = fornecedorRepository;
        _fornecedorService = fornecedorService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
    {
        var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
        return fornecedores;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
    {
        var fornecedor = await ObterFornecedorProdutosEndereco(id);

        if (fornecedor == null) return NotFound();

        return fornecedor;
    }

    [HttpPost]
    public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
    {
        if (!ModelState.IsValid) return BadRequest();

        var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
        var result = await _fornecedorService.Adicionar(fornecedor);

        if (!result) return BadRequest();

        return Ok(await ObterFornecedorEndereco(fornecedor.Id));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
    {
        if (id != fornecedorViewModel.Id) return BadRequest();

        if (!ModelState.IsValid) return BadRequest();

        var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
        var result = await _fornecedorService.Atualizar(fornecedor);

        if (!result) return BadRequest();

        return Ok(fornecedor);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
    {
        var fornecedorViewModel = await ObterFornecedorEndereco(id);

        if (fornecedorViewModel == null) return NotFound();

        var result = await _fornecedorService.Remover(id);

        if (!result) return BadRequest();

        return NoContent();
    }

    private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
    }

    private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
    }
}