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
using System.Threading.Tasks;

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

        [Theory]
        [InlineData(Freshness.Fresh)]
        [InlineData(Freshness.Expired)]
        [InlineData(Freshness.ExpiringToday)]
        public async void VerifyGetBatches(Freshness freshness)
        {
            Batch matchingBatch = getBatch(testProduct1.Id, freshness);

            Freshness nonMatchingFreshness = (freshness == Freshness.Fresh) ? Freshness.ExpiringToday : Freshness.Fresh;
            Batch nonMatchingBach = getBatch(testProduct1.Id, nonMatchingFreshness);

            List<Batch> list = new List<Batch>();
            list.Add(matchingBatch);
            list.Add(nonMatchingBach);

            mockDataHandler.Setup(m => m.GetData<Batch>(DataSource.Batches))
                .ReturnsAsync(list);

            IEnumerable<Batch> products = await dataAccess.GetBatches(freshness);

            Assert.True(products.Count() == 1);

            Batch b = products.First();
            Assert.Equal(testProduct1.Id, b.ProductId);
            Assert.Equal(matchingBatch.Id, b.Id);

            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Products), Times.Never);
            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Batches), Times.Once);
            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.BatchEvents), Times.Never);
        }

        [Theory]
        [InlineData(BatchEventType.Added)]
        [InlineData(BatchEventType.DisposedOf)]
        [InlineData(BatchEventType.Emptied)]
        [InlineData(BatchEventType.PortionsRemoved)]
        public async void VerifyGetBatchHistory(BatchEventType type)
        {
            BatchEvent e = BatchEvent.GetInstance(testBatch1, type);
            List<BatchEvent> list = new List<BatchEvent>();
            list.Add(e);

            mockDataHandler.Setup(m => m.GetData<BatchEvent>(DataSource.BatchEvents)).ReturnsAsync(list);

            IEnumerable<BatchEvent> events = await dataAccess.GetBatchHistory(testBatch1.Id);
            Assert.True(events.Count() == 1);

            BatchEvent ev = events.First();
            Assert.Equal(testBatch1.Id, ev.BatchId);

            mockDataHandler.Verify(m => m.GetData<BatchEvent>(DataSource.Products), Times.Never);
            mockDataHandler.Verify(m => m.GetData<BatchEvent>(DataSource.Batches), Times.Never);
            mockDataHandler.Verify(m => m.GetData<BatchEvent>(DataSource.BatchEvents), Times.Once);
        }

        [Fact]
        public async void VerifyAddBatch()
        {
            int size = 888;
            DateTime exp = DateTime.UtcNow.AddDays(7);

            mockDataHandler.Setup(m => m.GetData<Batch>(DataSource.Batches))
                .ReturnsAsync(batchList);

            mockDataHandler.Setup(
                m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>())
            ).Returns(Task.CompletedTask);

            mockDataHandler.Setup(
                m => m.GetData<BatchEvent>(DataSource.BatchEvents)
                ).ReturnsAsync(new List<BatchEvent>());

            mockDataHandler.Setup(
                m => m.WriteData(DataSource.BatchEvents, It.IsAny<List<BatchEvent>>())
            ).Returns(Task.CompletedTask);

            Batch newBatch = await dataAccess.AddBatch(testProduct1.Id, size, exp);

            Assert.Equal(size, newBatch.BatchSize);
            Assert.Equal(exp, newBatch.Expiration);

            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Batches), Times.Once);
            mockDataHandler.Verify(m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>()), Times.Once);
            mockDataHandler.Verify(m => m.GetData<BatchEvent>(DataSource.BatchEvents), Times.Once);
            mockDataHandler.Verify(m => m.WriteData(DataSource.BatchEvents, It.IsAny<List<BatchEvent>>()), Times.Once);
        }

        [Fact]
        public async void VerifyRemoveFromBatch()
        {
            int initialSize = testBatch1.BatchSize;
            int change = 10;

            mockDataHandler.Setup(m => m.GetData<Batch>(DataSource.Batches))
                .ReturnsAsync(batchList);

            mockDataHandler.Setup(
                m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>())
            ).Returns(Task.CompletedTask);

            mockDataHandler.Setup(
                m => m.GetData<BatchEvent>(DataSource.BatchEvents)
                ).ReturnsAsync(new List<BatchEvent>());

            mockDataHandler.Setup(
                m => m.WriteData(DataSource.BatchEvents, It.IsAny<List<BatchEvent>>())
            ).Returns(Task.CompletedTask);

            Batch udpatedBatch = await dataAccess.RemoveFromBatch(testBatch1.Id, change);

            Assert.Equal(initialSize - change, udpatedBatch.AvailableQuantity);

            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Batches), Times.Once);
            mockDataHandler.Verify(m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>()), Times.Once);
            mockDataHandler.Verify(m => m.GetData<BatchEvent>(DataSource.BatchEvents), Times.Once);
            mockDataHandler.Verify(m => m.WriteData(DataSource.BatchEvents, It.IsAny<List<BatchEvent>>()), Times.Once);
        }

        [Fact]
        public async void VerifyFixExpirationDate()
        {
            DateTime fixedExpiration = DateTime.UtcNow.AddDays(30);

            mockDataHandler.Setup(m => m.GetData<Batch>(DataSource.Batches))
                .ReturnsAsync(batchList);

            mockDataHandler.Setup(
                m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>())
            ).Returns(Task.CompletedTask);

            Batch udpatedBatch = await dataAccess.FixExpirationDate(testBatch1.Id, fixedExpiration);
            Assert.Equal(fixedExpiration, udpatedBatch.Expiration);

            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Batches), Times.Once);
            mockDataHandler.Verify(m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>()), Times.Once);
        }

        [Fact]
        public async void VerifyFixQuantities()
        {
            int updatedSize = 1444;
            int updatedAvailableQuantity = 1111;

            mockDataHandler.Setup(m => m.GetData<Batch>(DataSource.Batches))
                .ReturnsAsync(batchList);

            mockDataHandler.Setup(
                m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>())
            ).Returns(Task.CompletedTask);

            Batch udpatedBatch = await dataAccess.FixQuantities(testBatch1.Id, updatedSize, updatedAvailableQuantity);
            Assert.Equal(updatedSize, udpatedBatch.BatchSize);
            Assert.Equal(updatedAvailableQuantity, udpatedBatch.AvailableQuantity);

            mockDataHandler.Verify(m => m.GetData<Batch>(DataSource.Batches), Times.Once);
            mockDataHandler.Verify(m => m.WriteData(DataSource.Batches, It.IsAny<List<Batch>>()), Times.Once);
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

        private Batch getBatch(Guid productId, Freshness freshness)
        {
            Batch b = Batch.GetInstance(productId, 1000);

            if (freshness == Freshness.Expired)
            {
                b.Expiration = DateTime.UtcNow.AddDays(-7);
            }
            else if (freshness == Freshness.ExpiringToday)
            {
                b.Expiration = DateTime.UtcNow.AddHours(6);
            }
            else
            {
                b.Expiration = DateTime.UtcNow.AddDays(7);
            }

            return b;
        }
    }
}
