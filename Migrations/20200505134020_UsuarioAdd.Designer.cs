﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjetoVSCApi.Data;

namespace ProjetoVSCApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200505134020_UsuarioAdd")]
    partial class UsuarioAdd
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ProjetoVSCApi.Models.Produto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Nome");

                    b.Property<double>("Preco");

                    b.HasKey("Id");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("ProjetoVSCApi.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Senha");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
