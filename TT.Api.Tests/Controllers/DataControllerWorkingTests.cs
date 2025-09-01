using Microsoft.AspNetCore.Mvc;
using System;
using TT.Api.Controllers;
using Xunit;
using System.Reflection;

namespace TT.Api.Tests.Controllers
{
    /// Working unit tests for DataController - focusing on attributes and structure
    public class DataControllerWorkingTests
    {
        [Fact]
        public void DataController_HasCorrectRouting()
        {
            // Arrange
            var controllerType = typeof(DataController);

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(controllerType, typeof(RouteAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("[controller]", routeAttribute.Template);
        }

        [Fact]
        public void DataController_HasApiControllerAttribute()
        {
            // Arrange
            var controllerType = typeof(DataController);

            // Act
            var apiControllerAttribute = Attribute.GetCustomAttribute(controllerType, typeof(ApiControllerAttribute));

            // Assert
            Assert.NotNull(apiControllerAttribute);
        }

        [Fact]
        public void GetProducts_HasCorrectRoute()
        {
            // Arrange
            var method = typeof(DataController).GetMethod("GetProducts");

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            var httpGetAttribute = (HttpGetAttribute)Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("products", routeAttribute.Template);
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void GetProperties_HasCorrectRoute()
        {
            // Arrange
            var method = typeof(DataController).GetMethod("GetProperties");

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            var httpGetAttribute = (HttpGetAttribute)Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("properties", routeAttribute.Template);
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void TestConnection_HasCorrectRoute()
        {
            // Arrange
            var method = typeof(DataController).GetMethod("TestConnection");

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            var httpGetAttribute = (HttpGetAttribute)Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("test-connection", routeAttribute.Template);
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void CheckSchema_HasCorrectRoute()
        {
            // Arrange
            var method = typeof(DataController).GetMethod("CheckSchema");

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            var httpGetAttribute = (HttpGetAttribute)Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("schema-check", routeAttribute.Template);
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void DataController_HasCorrectNamespace()
        {
            // Arrange
            var controllerType = typeof(DataController);

            // Act & Assert
            Assert.Equal("TT.Api.Controllers", controllerType.Namespace);
        }

        [Fact]
        public void DataController_InheritsFromControllerBase()
        {
            // Arrange
            var controllerType = typeof(DataController);

            // Act & Assert
            Assert.True(controllerType.IsSubclassOf(typeof(ControllerBase)));
        }

        [Fact]
        public void DataController_HasRequiredMethods()
        {
            // Arrange
            var controllerType = typeof(DataController);

            // Act
            var productsMethod = controllerType.GetMethod("GetProducts");
            var propertiesMethod = controllerType.GetMethod("GetProperties");
            var testConnectionMethod = controllerType.GetMethod("TestConnection");
            var schemaCheckMethod = controllerType.GetMethod("CheckSchema");

            // Assert
            Assert.NotNull(productsMethod);
            Assert.NotNull(propertiesMethod);
            Assert.NotNull(testConnectionMethod);
            Assert.NotNull(schemaCheckMethod);
        }
    }
}