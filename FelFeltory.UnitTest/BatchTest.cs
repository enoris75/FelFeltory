using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FelFeltory.Models;

namespace FelFeltory.UnitTest
{
    [TestClass]
    public class BatchTest
    {
        [TestMethod]
        public void VerifyIsFresh()
        {
            Batch b = new Batch();
            b.Expiration = DateTime.UtcNow.AddDays(10);

            Freshness f = b.Freshness;

            Assert.IsTrue(f == Freshness.Fresh);
        }

        public void VerifyExpiresToday()
        {
            Batch b = new Batch();
            b.Expiration = DateTime.UtcNow.AddHours(10);

            Freshness f = b.Freshness;

            Assert.IsTrue(f == Freshness.ExpiringToday);
        }

        public void VerifyExpired()
        {
            Batch b = new Batch();
            b.Expiration = DateTime.UtcNow.AddDays(-1);

            Freshness f = b.Freshness;

            Assert.IsTrue(f == Freshness.Expired);
        }
    }
}
