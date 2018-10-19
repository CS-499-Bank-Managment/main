using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CS_499_Project
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc(routes =>
                {
                    routes.MapRoute("Teller-Test", "/Transaction/{acct_to}/{amount}", new
                    {
                        controller = "Teller",
                        Action = "Transaction"
                    });

                    routes.MapRoute("Login-Test", "/Login/", new
                    {
                        controller = "Login", action = "Index"
                    });

                    routes.MapRoute("Testing", "/Test", new
                        {controller = "Login", action = "Test"});

                    routes.MapRoute("Teller-TransferTest", "/Transfer/{acct_to}/{acct_from}/{amount}",
                        new {controller = "Teller", action = "Between"});
                    routes.MapRoute("creation", "/Admin/CustAcct/{username}",
                        new {controller = "Admin", action = "CustAcct"});
                    routes.MapRoute("acct_del", "/Admin/Del/{username}/{acct_id}",
                        new {controller = "Admin", action = "DeleteCustAcct"});
                    routes.MapRoute("admin", "{controller}/{action}/{username?}/{password?}/{role?}");
                    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    routes.MapRoute("index page", "/", new {controller = "User", action = "Index"});
                //https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.1
                //https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-a-view
                //https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-a-controller
            }
            );

            app.Run(async (context) =>
            {
                
            });
        }
    }
}
