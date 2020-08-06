using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FelFeltory.DataModels;
using System.Collections.Generic;

namespace FelFeltory.UnitTest
{
    [TestClass]
    public class OverviewByFreshnessTest
    {
        [TestMethod]
        public void VerifyAddFreshBatchesToOverview()
        {
            OverviewByFreshness overview = new OverviewByFreshness();
            Batch b = Batch.GetInstance(Guid.NewGuid(), 400);
            b.Expiration = DateTime.UtcNow.AddDays(10);

            List<Batch> list = new List<Batch> { b };

            overview.AddBatchesToOverview(list);

            Assert.IsTrue(overview.FreshBatches == 1);
            Assert.IsTrue(overview.FreshPortions == b.AvailableQuantity);
        }
        [TestMethod]
        public void VerifyAddExpiredBatchesToOverview()
        {
            OverviewByFreshness overview = new OverviewByFreshness();
            Batch b = Batch.GetInstance(Guid.NewGuid(), 400);
            b.Expiration = DateTime.UtcNow.AddDays(-10);

            List<Batch> list = new List<Batch> { b };

            overview.AddBatchesToOverview(list);

            Assert.IsTrue(overview.ExpiredBatches == 1);
            Assert.IsTrue(overview.ExpiredPortions == b.AvailableQuantity);
        }
        [TestMethod]
        public void VerifyAddExpiringBatchesToOverview()
        {
            OverviewByFreshness overview = new OverviewByFreshness();
            Batch b = Batch.GetInstance(Guid.NewGuid(), 400);
            b.Expiration = DateTime.UtcNow.AddHours(10);

            List<Batch> list = new List<Batch> { b };

            overview.AddBatchesToOverview(list);

            Assert.IsTrue(overview.ExpiringTodayBatches == 1);
            Assert.IsTrue(overview.ExpiringTodayPortions == b.AvailableQuantity);
        }
    }
}
