using System;

namespace WebApi.Infrastructure.Dto.Request
{
    public class TranslateRequest
    {
        public Guid Product { get; set; }

        public Guid Origin { get; set; }

        public Guid Destino { get; set; }

        public int Stock { get; set; }
    }
}
