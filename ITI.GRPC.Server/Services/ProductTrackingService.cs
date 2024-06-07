using Grpc.Core;
using ITI.GRPC.Server.Protos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITI.GRPC.Server.Services
{
    public class ProductTrackingService : InventoryServiceProto.InventoryServiceProtoBase
    {
        private static List<Product> products = new List<Product>();

        public override Task<IsExisted> GetProductById(Id request, ServerCallContext context)
        {
            var product = products.FirstOrDefault(p => p.Id == request.Id);
            return Task.FromResult(new IsExisted
            {
                Exists = product != null,
                Product = product
            });
        }

        public override Task<Product> AddProduct(Product request, ServerCallContext context)
        {
            products.Add(request);
            return Task.FromResult(request);
        }

        public override Task<Product> UpdateProduct(Product request, ServerCallContext context)
        {
            var existingProduct = products.FirstOrDefault(p => p.Id == request.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = request.Name;
                existingProduct.Quantity = request.Quantity;
                existingProduct.Price = request.Price;
                existingProduct.Category = request.Category;
            }
            return Task.FromResult(existingProduct ?? request);
        }

        public override Task<Products> GetAll(google.protobuf.Empty request, ServerCallContext context)
        {
            var response = new Products();
            response.Products_.AddRange(products);
            return Task.FromResult(response);
        }

        public override async Task<ProductsNumber> AddBulkProducts(IAsyncStreamReader<Product> requestStream, ServerCallContext context)
        {
            var count = 0;
            await foreach (var product in requestStream.ReadAllAsync())
            {
                products.Add(product);
                count++;
            }
            return new ProductsNumber { Number = count };
        }

        public override async Task GetProductReport(google.protobuf.Empty request, IServerStreamWriter<Product> responseStream, ServerCallContext context)
        {
            foreach (var product in products)
            {
                await responseStream.WriteAsync(product);
            }
        }
    }
}
