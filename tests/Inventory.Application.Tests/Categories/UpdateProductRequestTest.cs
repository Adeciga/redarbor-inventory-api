using FluentAssertions;
using Inventory.Application.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Tests.Categories
{
    public class UpdateProductRequestTest
    {
        [Fact]
        public void UpdateCategoryRequest_should_set_properties()
        {
            var request = new UpdateCategoryRequest(1, "Name", true);

            request.Id.Should().Be(1);
            request.Name.Should().Be("Name");
            request.IsActive.Should().BeTrue();
        }
    }
}
