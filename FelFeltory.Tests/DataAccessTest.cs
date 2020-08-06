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
        private readonly Mock<IDataAccessService> mockAccessService;
        private readonly InventoryController controller;

        private readonly Batch testBatch1;
        private readonly List<Batch> batchList;
        private readonly BatchEvent testEvent1;
        private readonly List<BatchEvent> eventList;

        public DataAccessTest()
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
    }
}
