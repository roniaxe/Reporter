﻿using System.Collections.Generic;

namespace Reporter.Data.Repositories
{
    public interface IGenericRepository<T>
    {
        List<T> GetAll();

        T GetById(object obj);

        T Insert(T obj);

        void Update(T obj);
    }
}