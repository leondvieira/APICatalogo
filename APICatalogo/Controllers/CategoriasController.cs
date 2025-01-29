using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IUnityOfWork _uof;
        private readonly ILogger _logger;

        public CategoriasController(ILogger<CategoriasController> logger, IUnityOfWork uof)
        {
            _logger = logger;
            _uof = uof;
        }

        private IEnumerable<CategoriaDTO> ObterCategorias(PagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious,
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ToCategoriaDTOList();
            return categoriasDto;
        }

        [HttpGet("categorias")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync()
        {
            var categorias = await _uof.CategoriaRepository.GetAllAsync();
            if (categorias == null)
                return NotFound("Não existem categorias...");

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet("categorias/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

            IEnumerable<CategoriaDTO> categoriasDto = ObterCategorias(categorias);
            return Ok(categoriasDto);

        }

        [HttpGet("categorias/filter/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradasAsync([FromQuery] CategoriasFiltroNome categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasParameters);
            IEnumerable<CategoriaDTO> categoriasDto = ObterCategorias(categorias);
            return Ok(categoriasDto);

        }



        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> GetAsync(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound("Categoria não encontrada...");
            }

            var categoriaDto = categoria.ToCategoriaDTO();

            return categoriaDto;
        }

        [HttpPost("categorias")]
        public async Task<ActionResult<CategoriaDTO>> PostAsync(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
            {
                return BadRequest();
            }

            var categoria = categoriaDto.ToCategoria();

            var categoria_criada = _uof.CategoriaRepository.Create(categoria);
            await _uof.CommitAsync();

            var novaCategoriaDto = categoria.ToCategoriaDTO();

            return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> PutAsync(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                _logger.LogWarning("Id da Categoria incorreto");
                return BadRequest();
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
            await _uof.CommitAsync();

            var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();


            return Ok(categoriaAtualizadaDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> DeleteAsync(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogWarning("Categoria não localizado...");
                return NotFound("Categoria não localizado...");
            }
            var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
            await _uof.CommitAsync();

            var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

            return Ok(categoriaExcluidaDto);
        }
    }
}
