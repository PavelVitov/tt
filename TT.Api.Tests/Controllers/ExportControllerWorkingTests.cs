using Microsoft.AspNetCore.Mvc;
using System;
using TT.Api.Controllers;
using Xunit;
using System.Reflection;

namespace TT.Api.Tests.Controllers
{
    /// <summary>
    /// Working unit tests for ExportController - focusing on attributes and structure
    /// </summary>
    public class ExportControllerWorkingTests
    {
        [Fact]
        public void ExportController_HasCorrectRouting()
        {
            // Arrange
            var controllerType = typeof(ExportController);

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(controllerType, typeof(RouteAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("[controller]", routeAttribute.Template);
        }

        [Fact]
        public void ExportController_HasApiControllerAttribute()
        {
            // Arrange
            var controllerType = typeof(ExportController);

            // Act
            var apiControllerAttribute = Attribute.GetCustomAttribute(controllerType, typeof(ApiControllerAttribute));

            // Assert
            Assert.NotNull(apiControllerAttribute);
        }

        [Fact]
        public void ExportProductBrand_HasCorrectRoute()
        {
            // Arrange
            var method = typeof(ExportController).GetMethod("ExportProductBrand");

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            var httpGetAttribute = (HttpGetAttribute)Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("brand/{key}", routeAttribute.Template);
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void GetAvailableProducts_HasCorrectRoute()
        {
            // Arrange
            var method = typeof(ExportController).GetMethod("GetAvailableProducts");

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            var httpGetAttribute = (HttpGetAttribute)Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("products", routeAttribute.Template);
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void FindProperties_HasCorrectRoute()
        {
            // Arrange
            var method = typeof(ExportController).GetMethod("FindProperties");

            // Act
            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            var httpGetAttribute = (HttpGetAttribute)Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute));

            // Assert
            Assert.NotNull(routeAttribute);
            Assert.Equal("find-properties", routeAttribute.Template);
            Assert.NotNull(httpGetAttribute);
        }

        [Fact]
        public void ExportController_HasCorrectNamespace()
        {
            // Arrange
            var controllerType = typeof(ExportController);

            // Act & Assert
            Assert.Equal("TT.Api.Controllers", controllerType.Namespace);
        }

        [Fact]
        public void ExportController_InheritsFromControllerBase()
        {
            // Arrange
            var controllerType = typeof(ExportController);

            // Act & Assert
            Assert.True(controllerType.IsSubclassOf(typeof(ControllerBase)));
        }

        [Fact]
        public void ExportController_HasRequiredMethods()
        {
            // Arrange
            var controllerType = typeof(ExportController);

            // Act
            var exportMethod = controllerType.GetMethod("ExportProductBrand");
            var productsMethod = controllerType.GetMethod("GetAvailableProducts");
            var propertiesMethod = controllerType.GetMethod("FindProperties");

            // Assert
            Assert.NotNull(exportMethod);
            Assert.NotNull(productsMethod);
            Assert.NotNull(propertiesMethod);
        }
    }
}