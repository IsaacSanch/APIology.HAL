using Apiology.Hal;
using Apiology.Hal.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Apiology.Tests.HAL {


    public class HalModelTests {
        private readonly int expectedNumber = 1;
        private readonly string expectedHello = "world";
        private readonly object expectedComplex = new {
            foo = "bar"
        };

        private readonly object model = null;

        private readonly IEnumerable<HalLink> modelLinks = new List<HalLink> {
            new HalLink(HalLink.RelForSelf, "test"),
            new HalLink("number", "number/{number}"),
            new HalLink("operations", "operations/plus"),
            new HalLink("operations", "operations/minus")
        };

        public HalModelTests() {
            model = new {
                number = expectedNumber,
                hello = expectedHello,
                complex = expectedComplex
            };
        }

        [Fact]
        public void To_HAL_Model_With_No_Links() {
            dynamic HalModel = new HalModel(model);

            AssertModelProperties(HalModel);
            Assert.Empty(HalModel._links);
        }

        [Fact]
        public void To_HAL_Model_With_Links() {
            var HalModel = new HalModel(model);

            HalModel.AddLinks(modelLinks);

            AssertModelProperties(HalModel);
            AssertModelLinks(HalModel);
        }

        [Fact]
        public void To_HAL_Model_With_Embedded_No_Links() {
            var HalModel = new HalModel(model);

            var embeddedList = new List<object> { model };

            HalModel.AddEmbeddedCollection("one", embeddedList, null);

            dynamic dyn = HalModel;

            AssertModelProperties(HalModel);
            Assert.Empty(dyn._links);

            var embedded = dyn._embedded as Dictionary<string, IEnumerable<HalModel>>;

            Assert.NotNull(embedded);

            var embeddedCollection = embedded.SingleOrDefault();

            Assert.NotNull(embeddedCollection);
            Assert.Equal("one", embeddedCollection.Key);

            dynamic embeddedModel = embeddedCollection.Value.SingleOrDefault();

            Assert.NotNull(embeddedModel);
            AssertModelProperties(embeddedModel);
            Assert.Empty(embeddedModel._links);
        }

        [Fact]
        public void To_HAL_Model_With_Embedded_With_Links() {
            var HalModel = new HalModel(model);

            var embeddedList = new List<object> { model };

            HalModel.AddEmbeddedCollection("one", embeddedList, modelLinks);

            AssertModelProperties(HalModel);

            dynamic dyn = HalModel;

            var embedded = dyn._embedded as Dictionary<string, IEnumerable<HalModel>>;

            Assert.NotNull(embedded);

            var embeddedCollection = embedded.SingleOrDefault();

            Assert.NotNull(embeddedCollection);
            Assert.Equal("one", embeddedCollection.Key);

            dynamic embeddedModel = embeddedCollection.Value.SingleOrDefault();

            Assert.NotNull(embeddedModel);
            AssertModelProperties(embeddedModel);
            AssertModelLinks(embeddedModel);
        }

        private void AssertModelProperties(dynamic HalModel) {
            Assert.Equal(expectedNumber, HalModel.Dto.number);
            Assert.Equal(expectedHello, HalModel.Dto.hello);
            Assert.Equal(expectedComplex, HalModel.Dto.complex);
        }

        private static void AssertModelLinks(dynamic dyn) {
            var links = dyn._links as Dictionary<string, object>;

            Assert.NotNull(links);

            var selfLink = links[HalLink.RelForSelf] as HalLink;

            Assert.NotNull(selfLink);
            Assert.Equal(HalLink.RelForSelf, selfLink.Rel);
            Assert.Equal("test", selfLink.Href);

            var numberLink = links["number"] as HalLink;
            Assert.NotNull(numberLink);
            Assert.Equal("number", numberLink.Rel);
            Assert.Equal("number/1", numberLink.Href);


            var operationLinks = links["operations"] as IEnumerable<HalLink>;
            Assert.NotNull(operationLinks);
            Assert.Equal(2, operationLinks.Count());
            Assert.Contains(operationLinks, (l) => l.Href == "operations/plus");
            Assert.Contains(operationLinks, (l) => l.Href == "operations/minus");
        }
    }
}

