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

    public class InventoryControllerTest
    {
        private readonly Mock<IDataAccessService> mockAccessService;
        private readonly InventoryController controller;

        private readonly Batch testBatch1;
        private readonly List<Batch> batchList;
        private readonly BatchEvent testEvent1;
        private readonly List<BatchEvent> eventList;

        public InventoryControllerTest()
        {
            this.mockAccessService =
                new Mock<IDataAccessService>(MockBehavior.Strict);

            this.controller = new InventoryController(
                    new NullLogger<InventoryController>(),
                    this.mockAccessService.Object
                );

            testBatch1 = Batch.GetInstance(Guid.NewGuid(), 1000);
            batchList = new List<Batch>();
            batchList.Add(testBatch1);

            testEvent1 = BatchEvent.GetInstance(testBatch1, BatchEventType.Added);
            eventList = new List<BatchEvent>();
            eventList.Add(testEvent1);
        }

        [Fact]
        public async void VerifyGetAllBatches()
        {
            mockAccessService.Setup(a => a.GetAllBatches())
                .ReturnsAsync(batchList);

            ActionResult actionResult =
                await this.controller.GetAllBatches();
            OkObjectResult objResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(200, objResult.StatusCode);
            mockAccessService.Verify(a => a.GetAllBatches(), Times.Once);
        }

        [Theory]
        [InlineData(Freshness.Fresh)]
        [InlineData(Freshness.Expired)]
        [InlineData(Freshness.ExpiringToday)]
        public async void VerifyGetBatchesByFreshness(Freshness freshness)
        {
            mockAccessService.Setup(a => a.GetBatches(freshness))
                .ReturnsAsync(batchList);

            ActionResult actionResult = await this.controller.GetBatchesByFreshness(freshness);
            OkObjectResult objResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(200, objResult.StatusCode);
            mockAccessService.Verify(a => a.GetBatches(freshness), Times.Once);
        }

        [Fact]
        public async void VerifyGetBatchHistory()
        {
            Guid batchId = testBatch1.Id;
            mockAccessService.Setup(a => a.GetBatchHistory(batchId))
                .ReturnsAsync(eventList);

            ActionResult actionResult = await this.controller.GetBatchHistory(batchId);
            OkObjectResult objResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(200, objResult.StatusCode);
            mockAccessService.Verify(a => a.GetBatchHistory(batchId), Times.Once);
        }

        [Fact]
        public async void VerifyAddBatch()
        {
            AddBatchRequestBody request = new AddBatchRequestBody();
            request.ProductId = testBatch1.ProductId;
            request.BatchSize = testBatch1.BatchSize;
            request.ExpirationDate = testBatch1.Expiration;

            mockAccessService.Setup(
                a => a.AddBatch(
                    request.ProductId,
                    request.BatchSize,
                    request.ExpirationDate
                )
            ).ReturnsAsync(testBatch1);

            ActionResult actionResult = await this.controller.AddBatch(request);
            OkObjectResult objResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(200, objResult.StatusCode);
            mockAccessService.Verify(a => a.AddBatch(
                    request.ProductId,
                    request.BatchSize,
                    request.ExpirationDate
                ), Times.Once);
        }

        [Fact]
        public async void VerifyRemoveFromBatchWorks()
        {
            Guid batchId = testBatch1.Id;
            mockAccessService.Setup(
                a => a.RemoveFromBatch(
                    It.IsAny<Guid>(),
                    It.IsAny<int>()
                    )
                )
                .ReturnsAsync(testBatch1);

            ActionResult actionResult =
                await this.controller.RemoveFromBatch(batchId, 10);
            OkObjectResult objResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(200, objResult.StatusCode);
            mockAccessService.Verify(
                a => a.RemoveFromBatch(
                    It.IsAny<Guid>(),
                    It.IsAny<int>()
                    )
                , Times.Once
            );
        }

        [Fact]
        public async void VerifyRemoveFromBatchNegativeQuantity()
        {
            Guid batchId = testBatch1.Id;
            mockAccessService.Setup(
                a => a.RemoveFromBatch(
                    It.IsAny<Guid>(),
                    It.IsAny<int>()
                    )
                )
                .ReturnsAsync(testBatch1);

            ActionResult actionResult =
                await this.controller.RemoveFromBatch(batchId, -10);
            BadRequestObjectResult objResult =
                Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal(400, objResult.StatusCode);
            mockAccessService.Verify(
                a => a.RemoveFromBatch(
                    It.IsAny<Guid>(),
                    It.IsAny<int>()
                    )
                , Times.Never
            );
        }

        [Fact]
        public async void VerifyFixExpirationDate()
        {
            Guid batchId = testBatch1.Id;
            DateTime newExpirationDate = DateTime.UtcNow;

            mockAccessService.Setup(
                a => a.FixExpirationDate(batchId, newExpirationDate))
                .ReturnsAsync(testBatch1);

            ActionResult actionResult =
                await this.controller.FixExpirationDate(batchId, newExpirationDate);
            OkObjectResult objResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(200, objResult.StatusCode);
            mockAccessService.Verify(
                a => a.FixExpirationDate(batchId, newExpirationDate),
                Times.Once);
        }
    }
}
