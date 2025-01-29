﻿using APICatalogo.Context;

namespace APICatalogo.Repositories
{
    public class UnityOfWork : IUnityOfWork
    {
        private IProdutoRepository? _produtoRepository;

        private ICategoriaRepository? _categoriaRepository;

        public AppDbContext _context;


        public UnityOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepository = _produtoRepository ?? new ProdutoRepository(_context);
            }
        }

        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return _categoriaRepository = _categoriaRepository ?? new CategoriaRepository(_context);
            }
        }


        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
