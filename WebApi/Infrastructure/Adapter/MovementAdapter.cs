using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.enums;
using WebApi.Domain.Exceotion;
using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Dto.Request;
using WebApi.Infrastructure.Repository;

namespace WebApi.Infrastructure.Adapter
{
    public class MovementAdapter : GenericAdapter<Movement>, IMovementService
    {
        private readonly int maxCapicityOfProduct = 10000;
        private readonly AppDbContext context;
        private readonly IProductService productService;
        private readonly IStoreService storeService;
        private readonly IProductStoreService productStoreService;

        public MovementAdapter(AppDbContext dbContext, IProductService productService, IStoreService storeService,
            IProductStoreService productStoreService) : base(dbContext)
        {
            context = dbContext;
            this.productService = productService;
            this.storeService = storeService;
            this.productStoreService = productStoreService;
        }

        public async Task<Product> SaleOrBuyProducts(SaleBuy saleBuy, MovementType type)
        {
            var product = await GetProduct(saleBuy.Product);
            var store = await GetStore(saleBuy.Store);

            if (type == MovementType.OUTPUT)
            {
                await Sale(saleBuy.Stock, product, store);
            }
            else if (type == MovementType.INPUT)
            {
                await Buy(saleBuy.Stock, product, store);
            }

            await SaveAsync(new Movement { Product = product, Store = store, Stock = saleBuy.Stock, Type = type, MovementDate = DateTime.Now.ToUniversalTime() });
            return await GetProduct(saleBuy.Product);
        }

        public async Task TranslateProductStore(TranslateRequest translate)
        {
            var product = await GetProduct(translate.Product);
            var origin = await GetStore(translate.Origin);
            var dest = await GetStore(translate.Destino);

            ValidateMaximunStock(product);
            var productStoreOrigin = await FindAllByProductStore(translate.Product, translate.Origin);
            if (productStoreOrigin == null)
            {
                throw new BusinessException($"In the store {origin.Name} no found stock of product {product.Name}");
            }
            if (productStoreOrigin.Stock < translate.Stock)
            {
                throw new BusinessException($"In the store {origin.Name} no only stock of product {product.Name} is {productStoreOrigin.Stock}");
            }

            await Buy(translate.Stock, product, dest);

            productStoreOrigin.Stock -= translate.Stock;

            var idTranslate = Guid.NewGuid().ToString();
            var dateTranslate = DateTime.Now.ToUniversalTime();
            SaveNoSafe(new Movement { Product = product, Store = origin, Stock = translate.Stock, Type = MovementType.OUTPUT, MovementDate = dateTranslate, TranslateId = idTranslate });
            SaveNoSafe(new Movement { Product = product, Store = dest, Stock = translate.Stock, Type = MovementType.INPUT, MovementDate = dateTranslate, TranslateId = idTranslate });
            await SaveAsync();
        }

        private async Task Buy(int stock, Product product, Store store)
        {
            ValidateMaximunStock(product);
            var productStore = await FindAllByProductStore(product.Id, store.Id);
            if (productStore == null)
            {
                await CreateStockProduct(stock, product, store);
            }
            else
            {
                await UpdateStockBuyProduct(productStore, stock);
            }
        }

        private async Task Sale(int stock, Product product, Store store)
        {
            var productStore = await FindAllByProductStore(product.Id, store.Id);
            if (productStore == null)
            {
                throw new BusinessException($"In this store no stock of product {product.Name}");
            }
            else
            {
                if (productStore.Stock < stock)
                {
                    throw new BusinessException($"In this store {productStore.Store.Name} no stock of {stock} of product {product.Name} only stock {productStore.Stock}");
                }

                productStore.Stock -= stock;
            }
            await context.SaveChangesAsync();
        }

        private async Task<Product> GetProduct(Guid id)
        {
            return await productService.FindByIdAllData(id);
        }

        private async Task<Store> GetStore(Guid id)
        {
            return await storeService.FindByIdAllData(id);
        }

        private async Task CreateStockProduct(int stock, Product product, Store store)
        {
            if (store.MaxCapacity < stock)
            {
                throw new BusinessException($"In the Store {store.Name} the maximun capacity is {store.MaxCapacity}, of {stock}");
            }
            var productStore = new ProductStore
            {
                Id = Guid.NewGuid(),
                Product = product,
                Store = store,
                Stock = stock
            };
            await productStoreService.SaveAsync(productStore);
        }

        private async Task UpdateStockBuyProduct(ProductStore productStore, int stock)
        {
            var max = GetCapicity(productStore.Store);
            if (max < stock)
            {
                throw new BusinessException($"In the store {productStore.Store.Name} there is only capacity for {max}");
            }
            productStore.Stock += stock;
            await context.SaveChangesAsync();
        }

        private void ValidateMaximunStock(Product product)
        {
            var stockOfProduct = product.ProductStores.Sum(ps => ps.Stock);
            if (stockOfProduct >= maxCapicityOfProduct)
            {
                throw new BusinessException($"In the inventory there are already a total of {stockOfProduct} of Product {product.Name}");
            }
        }

        private async Task<ProductStore> FindAllByProductStore(Guid product, Guid store)
        {
            return await context.ProductStores.Include(ps => ps.Store)
                                                          .Include(ps => ps.Product)
                                                          .Where(ps => ps.Store.Id.Equals(store) && ps.Product.Id.Equals(product))
                                                          .FirstOrDefaultAsync();
        }

        private static long GetCapicity(Store store)
        {
            return store.MaxCapacity - store.ProductStores.Sum(ps => ps.Stock);
        }

    }
}
