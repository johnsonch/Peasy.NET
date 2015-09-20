﻿using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Facile.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Facile.Core.Tests.Extensions
{
    [Trait("Extensions", "IDomainObjectExtensionsTests")]
    public class IDomainObjectExtensionsTests
    {
        [Fact]
        public void GetValidationErrorsReturnsNoResults()
        {
            var person = new Person()
            {
                ID = 0,
                First = "Skip",
                Last = "Jones"
            };
            var results = person.GetValidationErrors();
            results.Count().ShouldBe(0);
        }

        [Fact]
        public void GetValidationErrorsReturnsOneResult()
        {
            var person = new Person()
            {
                ID = -1,
                First = "Skip",
                Last = "Jones"
            };
            var results = person.GetValidationErrors();
            results.Count().ShouldBe(1);
        }

        [Fact]
        public void GetValidationErrorsReturnsTwoResults()
        {
            var person = new Person()
            {
                ID = -1,
                First = "AnExtremelyLongFirstName",
                Last = "Jones"
            };
            var results = person.GetValidationErrors();
            results.Count().ShouldBe(2);
        }

        [Fact]
        public void GetValidationErrorsReturnsThreeResults()
        {
            var person = new Person()
            {
                ID = -1,
                First = "AnExtremelyLongFirstName",
                Last = "AnExtremelyLongLastNameAnExtremelyLongLastName"
            };
            var results = person.GetValidationErrors();
            results.Count().ShouldBe(3);
        }

        public class Person : IDomainObject
        {
            [Range(0, 50)]
            public int ID { get; set; }
            [MaxLength(15)]
            public string First { get; set; }
            [MaxLength(30)]
            public string Last { get; set; }
        }
    }
}