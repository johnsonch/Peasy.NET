﻿using AutoMapper;
using Orders.com.Core.DataProxy;
using Orders.com.Core.Domain;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Orders.com.DAL.Mock
{
    public class CategoryRepository : OrdersDotComMockBase<Category>, ICategoryDataProxy
    {
        protected override IEnumerable<Category> SeedRepository()
        {
            yield return new Category() { CategoryID = 1, Name = "Guitars" };
            yield return new Category() { CategoryID = 2, Name = "Computers" };
            yield return new Category() { CategoryID = 3, Name = "Albums" };
        }
    }
}
