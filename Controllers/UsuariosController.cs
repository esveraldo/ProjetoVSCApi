using System.Text;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjetoVSCApi.Data;
using ProjetoVSCApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;

namespace ProjetoVSCApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario){
            if(usuario.Email == "" || usuario.Email == null){
                Response.StatusCode = 400;
                return new ObjectResult("Status 400 - O email não pode ser nulo!");
            }

            if(usuario.Senha == "" || usuario.Senha == null){
                Response.StatusCode = 400;
                return new ObjectResult("Status 400 - A senha não pode ser nula!");
            }
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return Ok(new {msg = "Usuário usado com sucesso!"});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credenciais){
            try{
                Usuario usuario = _context.Usuarios.First(u => u.Email.Equals(credenciais.Email));
                if(credenciais.Email != null && usuario.Email.Equals(credenciais.Email)){
                    if(credenciais.Senha != null && usuario.Senha.Equals(credenciais.Senha)){
                        //CHAVE DE SEGURANÇA
                        string Chave = "chave_de_segurança";
                        var ChaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Chave));
                        var CredenciaisDeAcesso = new SigningCredentials(ChaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        //ADICIONANDO INFORMAÇÕES DO USUÁRIO NO TOKEN
                        var claims = new List<Claim>();
                        claims.Add(new Claim("id", usuario.Id.ToString()));
                        claims.Add(new Claim("email", usuario.Email));
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                        var JWT = new JwtSecurityToken(
                            //QUEM ESTÁ FORNECENDO O JWT PARA O USUÁRIO
                            issuer: "Esveraldo Martins", 
                            //TEMPO DE EXPIRAÇÃO DO TOKEN
                            expires: DateTime.Now.AddHours(1),
                            //TIPO DE USUÁRIO QUE VOCÊ VAI ENVIAR O TOKEN
                            audience: "usuario_comum",
                            signingCredentials: CredenciaisDeAcesso,
                            claims: claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                    }else{
                        Response.StatusCode = 401;
                        return new ObjectResult("Erro 401 - Não autorizado");
                    }
                }else{
                    Response.StatusCode = 401;
                    return new ObjectResult("Erro 401 - Não autorizado");
                }
            }catch(Exception e){
                Response.StatusCode = 401;
                return new ObjectResult("Erro 401 - Não autorizado " + e.Message);
            }
        }
    }
}