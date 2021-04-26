using System;

namespace WebApi.Infrastructure.Dto.Request
{
    public class SaleBuy
    {
        public Guid Product { get; set; }

        public Guid Store { get; set; }

        public int Stock { get; set; }

    }
}
