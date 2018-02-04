using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Environment.Navigation;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Security.Permissions;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Drivers;
using OrchardCore.Workflows.Evaluators;
using OrchardCore.Workflows.Expressions;
using OrchardCore.Workflows.Helpers;
using OrchardCore.Workflows.Indexes;
using OrchardCore.Workflows.Liquid;
using OrchardCore.Workflows.Services;
using OrchardCore.Workflows.WorkflowContextProviders;
using YesSql.Indexes;

namespace OrchardCore.Workflows
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddScoped(typeof(Resolver<>));
            services.AddScoped<ISecurityTokenService, SecurityTokenService>();
            services.AddScoped<IActivityLibrary, ActivityLibrary>();
            services.AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
            services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();
            services.AddScoped<IWorkflowManager, WorkflowManager>();
            services.AddScoped<IActivityDisplayManager, ActivityDisplayManager>();
            services.AddScoped<IDataMigration, Migrations>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IDisplayDriver<IActivity>, MissingActivityDisplay>();
            services.AddSingleton<IIndexProvider, WorkflowDefinitionIndexProvider>();
            services.AddSingleton<IIndexProvider, WorkflowInstanceIndexProvider>();
            services.AddScoped<IWorkflowExecutionContextHandler, DefaultWorkflowExecutionContextHandler>();
            services.AddScoped<IWorkflowExecutionContextHandler, SignalWorkflowExecutionContextHandler>();
            services.AddScoped<IWorkflowExpressionEvaluator, LiquidWorkflowExpressionEvaluator>();
            services.AddScoped<IWorkflowScriptEvaluator, JavaScriptWorkflowScriptEvaluator>();

            services.AddActivity<NotifyTask, NotifyTaskDisplay>();
            services.AddActivity<SetPropertyTask, SetVariableTaskDisplay>();
            services.AddActivity<SetOutputTask, SetOutputTaskDisplay>();
            services.AddActivity<CorrelateTask, CorrelateTaskDisplay>();
            services.AddActivity<ForkTask, ForkTaskDisplay>();
            services.AddActivity<JoinTask, JoinTaskDisplay>();
            services.AddActivity<ForLoopTask, ForLoopTaskDisplay>();
            services.AddActivity<WhileLoopTask, WhileLoopTaskDisplay>();
            services.AddActivity<IfElseTask, IfElseTaskDisplay>();
            services.AddActivity<ScriptTask, ScriptTaskDisplay>();
            services.AddActivity<LogTask, LogTaskDisplay>();
            services.AddActivity<SignalEvent, SignalEventDisplay>();

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<ILiquidTemplateEventHandler, SignalLiquidTemplateHandler>();
            services.AddLiquidFilter<SignalUrlFilter>("signal_url");
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaRoute(
                name: "ExecuteWorkflow",
                areaName: "OrchardCore.Workflows",
                template: "Workflows/{action}",
                defaults: new { controller = "Workflow" }
            );

            routes.MapAreaRoute(
                name: "SignalWorkflow",
                areaName: "OrchardCore.Workflows",
                template: "Workflows/Trigger",
                defaults: new { controller = "Signal", action = "Trigger" }
            );

            routes.MapAreaRoute(
                name: "AddActivity",
                areaName: "OrchardCore.Workflows",
                template: "Admin/Workflows/Definitions/{workflowDefinitionId}/Activity/{activityName}/Add",
                defaults: new { controller = "Activity", action = "Create" }
            );

            routes.MapAreaRoute(
                name: "EditActivity",
                areaName: "OrchardCore.Workflows",
                template: "Admin/Workflows/Definitions/{workflowDefinitionId}/Activity/{activityId}/Edit",
                defaults: new { controller = "Activity", action = "Edit" }
            );

            routes.MapAreaRoute(
                name: "WorkflowInstances",
                areaName: "OrchardCore.Workflows",
                template: "Admin/Workflows/Definitions/{workflowDefinitionId}/Instances/{action}",
                defaults: new { controller = "WorkflowInstance", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "WorkflowDefinitions",
                areaName: "OrchardCore.Workflows",
                template: "Admin/Workflows/Definitions/{action}/{id?}",
                defaults: new { controller = "WorkflowDefinition", action = "Index" }
            );
        }
    }
}