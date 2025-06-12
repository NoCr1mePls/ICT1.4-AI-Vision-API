
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using HomeTry.Controllers;
using HomeTry.Repositories;
using Moq;

namespace ICT1._4_Api_Tests.Controllers
{
    [TestClass]
    public class MonitoringLitterControllerTest
    {
        private Mock<LitterRepository> _mockRepo;
        private MonitoringLitterController _mockController;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<LitterRepository>();
            _mockController = new MonitoringLitterController();
        }

        [TestMethod]
        public void TestMethod1()
        {

        }
    }
}


