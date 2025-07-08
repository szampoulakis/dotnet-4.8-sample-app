using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DescopeSampleApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private TracerProvider _tracerProvider;
        private MeterProvider _meterProvider;

        protected void Application_Start()
        {
            _tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddAspNetInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri("http://localhost:4318/v1/traces");
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                })
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: "DescopeSampleApp", serviceVersion: "1.0.0"))
                .Build();

            _meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddAspNetInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri("http://localhost:4318/v1/metrics");
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                })
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: "DescopeSampleApp", serviceVersion: "1.0.0"))
                .Build();
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            _tracerProvider?.Dispose();
            _meterProvider?.Dispose();
        }
    }
}