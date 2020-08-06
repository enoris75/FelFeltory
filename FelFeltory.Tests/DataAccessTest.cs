using System;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Moq;
using FelFeltory.Controllers;
using FelFeltory.DataAccess;
using FelFeltory.DataModels;
using System.Linq;
using Xunit;
using FelFeltory.RequestModels;

namespace FelFeltory.UnitTest
{

    public class DataAccessTest
    {
        private readonly Mock<IDataHandler> mockDataHandler;
        private readonly DataAccessService dataAccess;
        private readonly Product testProduct1;
        private readonly List<Product> productList;

        private readonly Batch testBatch1;
        private readonly List<Batch> batchList;

        public DataAccessTest()
        {
            mockDataHandler = new Mock<IDataHandler>(MockBehavior.Strict);
            dataAccess = new DataAccessService(mockDataHandler.Object);

            testProduct1 = getRandomProduct();
            productList = new List<Product>();
            productList.Add(testProduct1);

            testBatch1 = Batch.GetInstance(testProduct1.Id, 1000);
            batchList = new List<Batch>();
            batchList.Add(testBatch1);
        }

        [Fact]
        public async void VerifyGetAllProducts()
        {
            mockDataHandler.Setup(
                m => m.GetData<Product>(DataSource.Products)
            ).ReturnsAsync(productList);

            IEnumerable<Product> products = await dataAccess.GetAllProducts();

            Assert.True(products.Count() == 1);

            Product p = products.First();
            Assert.Equal(testProduct1.Id, p.Id);

            mockDataHandler.Verify(m => m.GetData<Product>(DataSource.Products), Times.Once);
            mockDataHandler.Verify(m => m.GetData<Product>(DataSource.Batches), Times.Never);
            mockDataHandler.Verify(m => m.GetData<Product>(DataSource.BatchEvents), Times.Never);
        }

        [Fact]
        public async void VerifyGetAllBatches()
        {
            mockDataHandler.Setup(m => m.GetData<Batch>(DataSource.Batches))
                .ReturnsAsync(batchList);

            IEnumerable<Batch> products = await dataAccess.GetAllBatches();

            Assert.True(products.Count() == 1);

            Batch b = products.First();
            Assert.Equal(testProduct1.Id, b.ProductId);

            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Products), Times.Never);
            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Batches), Times.Once);
            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.BatchEvents), Times.Never);
        }

        private Product getRandomProduct()
        {
            Product p = new Product();
            p.Id = Guid.NewGuid();

            Random rnd = new Random();
            string randomNumber = rnd.Next().ToString();
            p.Name = "I am a mock product " + randomNumber;
            p.Description = "I am the description of the mock Product " + randomNumber;

            return p;
        }
    }
}
