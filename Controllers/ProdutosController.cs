using System;
using Microsoft.AspNetCore.Mvc;
using ProjetoVSCApi.Data;
using ProjetoVSCApi.Models;
using System.Linq;
using ProjetoVSCApi.HATEOAS;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoVSCApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private HATEOAS.HATEOAS HATEOAS;
        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/Produtos");
            HATEOAS.AddAction("GET_INFO","GET");
            HATEOAS.AddAction("DELETE_PRODUCT","DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT","PATCH");
        }

        [HttpGet("teste")]
        public IActionResult TesteClaims(){
           return Ok(HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id",StringComparison.InvariantCultureIgnoreCase)).Value);
        }

        [HttpGet]
        public IActionResult List(){
            if(!_context.Produtos.Any()){
                Response.StatusCode = 404;
                return new ObjectResult("Http 404 - Not found!");
            }
            
            var produtos = _context.Produtos.ToList();

            List<ProdutoContainer> produtosHATEOAS = new List<ProdutoContainer>();
            foreach(var prod in produtos){
                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = prod;
                produtoHATEOAS.links = HATEOAS.GetActions(prod.Id.ToString());
                produtosHATEOAS.Add(produtoHATEOAS);
            }

            return Ok(produtosHATEOAS); 
        }

        [HttpGet("{id}")]
        public IActionResult Prod(int id){
            try{
                var produto = _context.Produtos.First(p => p.Id.Equals(id));

                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = produto;
                produtoHATEOAS.links= HATEOAS.GetActions(produto.Id.ToString());

                if(produto.Id != id){
                    Response.StatusCode = 404;
                    return new ObjectResult("Http 404 - Not found!");
                    //return Ok("Http 404 - Not found!");
                } 
                return Ok(produtoHATEOAS);
            }catch(Exception){
                return BadRequest(new {Erro = "Id não encontrado!"});
            }
        }

        [HttpPost]
        public IActionResult Gravar([FromBody]Produto produto){

            if(produto.Nome == "" || produto.Nome == null){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O produto não pode estar sem nome para cadastro!"});
            }

            if(produto.Preco <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O produto não pode ter um preço zero!"});
            }

            var prod = _context.Produtos.Add(produto);
            _context.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("Http 200 Ok!");
            //return Ok("Http 200 Ok!");
        }

        [HttpDelete("{id}")]
        public IActionResult Del(int id){
            try{
                var produto = _context.Produtos.First(p => p.Id.Equals(id));
                _context.Produtos.Remove(produto);
                _context.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Produto removido!"); 
            }catch(Exception e){
                Response.StatusCode = 404;
                return new ObjectResult("Id não encontrado!");
            }
        }

        [HttpPatch("{id}")]
        public IActionResult Editar(int id, [FromBody]Produto produto){
            try{

                var ProdId = _context.Produtos.First(p => p.Id.Equals(id));

                if(!ProdId.Equals(id) || id < 0){
                    Response.StatusCode = 400;
                    return new ObjectResult("Id do produto não encontrado!");
                }

                if(produto.Nome == "" || produto.Nome == null){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O produto não pode estar sem nome para cadastro!"});
                }

                if(produto.Preco <= 0){
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O produto não pode ter um preço zero!"});
                }

                ProdId.Nome = produto.Nome != null ? produto.Nome : ProdId.Nome;
                ProdId.Preco = produto.Preco <= 0 ? produto.Preco : ProdId.Preco; 

                _context.SaveChanges();
                return Ok("Produto alterado com sucesso!");
            }catch(Exception e){
                Response.StatusCode = 400;
                return  new ObjectResult("Houve um erro na sua requisição!");
            }
        }

        public class ProdutoContainer {
            public Produto produto;
            public Link[] links;
        }
    }
}