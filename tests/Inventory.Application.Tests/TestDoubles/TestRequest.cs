using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Tests.TestDoubles
{
    public sealed record TestRequest : IRequest<Unit>;
}
